using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Лекция_2_1_немигающие_шарики_
{
    public class Animator
    {
        private Circle c;
        private Thread? t = null;
        // Чтобы шарики не замерали
        public bool isAlive => t == null || t.IsAlive;
        // Размер панели, нужен для цикла анимации
        public Size ContainerSize { get; set; }
        public Animator(Size containerSize)
        {
            c = new Circle(50, 0, 0);
            ContainerSize = containerSize;
        }

        // Метод для перемещения шарика, запускает аниматор
        public void Start()
        {
            t = new Thread(() =>
            {
                while (c.X + c.Diam < ContainerSize.Width)
                {
                    // Ожидание пока шарик сдвинется
                    Thread.Sleep(30);
                    c.Move(1, 0);//сдвиг вправо
                }

            });
            t.IsBackground = true; // фоновый поток, все потоки автоматически закрываются
            t.Start();
        }

        // Рисуем шарик
        public void PaintCircle(Graphics g)
        {
            c.Paint(g);
        }
    }
}
