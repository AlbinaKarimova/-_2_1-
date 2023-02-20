using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Лекция_2_1_немигающие_шарики_
{
    // Отрисовка поля
    public class Painter
    {
        // locker должен быть во всех местах, где идет обращение к общим данным
        public object locker = new();
        // Список аниматоров
        private List<Animator> animators = new();
        private Size containerSize;
        private Thread t;
        private Graphics mainGraphics;
        // Буфферизованная графика
        private BufferedGraphics bg;
        private bool isAlive;
        public Thread PaintedThread => t;
        // счетчик нарисованных объектов(используем для немигания экрана)
        public /*volatile*/ int objectPainted = 0;
        public Graphics MainGraphics
        {
            get => mainGraphics;
            set
            {
                lock (locker)
                {
                    mainGraphics = value;
                    //узнаем размер контейнера(панели), свойство ToSize приводит к целоч-му типу значение Size
                    ContainerSize = mainGraphics.VisibleClipBounds.Size.ToSize();
                    bg = BufferedGraphicsManager.Current.Allocate(mainGraphics,
                        new Rectangle(new Point(0, 0), ContainerSize)
                        );
                    objectPainted = 0;
                }
            }
        }
        public Size ContainerSize
        {
            get => containerSize;
            set
            {
                containerSize = value;
                // чтобы шарики двигались дальше по панели, после изменения ее размера
                foreach(var animator in animators)
                {
                    animator.ContainerSize = ContainerSize;
                }
            }
        }
        
        public Painter(Graphics mainGraphics)
        {
            MainGraphics = mainGraphics;
        }

        // Метод для добавления нового аниматора,
        // при нажатии на кнопку
        public void AddNew()
        {
            var a = new Animator(ContainerSize);
            animators.Add(a);
            a.Start();
        }

        // Метод для запуска отдельного потока(отрисовываем сцену)
        public void Start()
        {
            isAlive = true;
            t = new Thread(() =>
            {
                try
                {
                    while (isAlive)
                    {
                        // Остановившиеся шарики будут исчезать
                        // Функция в качестве параметра принимает каждое значение
                        // коллекции(it) и выберает удалять или нет
                        animators.RemoveAll(it => !it.isAlive);
                      // lock блокирует другие потоки кроме данного
                      lock (locker)
                      {
                          if (PaintOnBuffer())
                          {
                          // Переносим изображение на панель
                             bg.Render(MainGraphics);
                          }
                      }
                    if (isAlive) Thread.Sleep(30);
                    // Частота кадров
                    //Thread.Sleep(30);
                    }
                }
                catch(ArgumentException e) { }
            });
            t.IsBackground = true;
            t.Start();
        }
        public void Stop()
        {
            isAlive = false;
            // Метод приводит к возникновению исключения, когда поток находится в замершем состоянии
            // Не работает в цикле
            //если поток ожидает чего-то или спить, то этим методом можно прервать ожидание
            t.Interrupt();
        }
        private bool PaintOnBuffer()
        {
            objectPainted = 0;
            var objectCount = animators.Count;
            bg.Graphics.Clear(Color.White);
            foreach (var animator in animators)
            {
                animator.PaintCircle(bg.Graphics); //bg-двойная буферизация
                objectPainted++;// увеличиваем счетчик на каждый нарисованный шарик
            }
            return objectPainted == objectCount;
        }
    }
}
