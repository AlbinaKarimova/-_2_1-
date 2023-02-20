namespace Лекция_2_1_немигающие_шарики_
{
    public partial class Form1 : Form
    {
        private Painter p;
        public Form1()
        {
            InitializeComponent();
            p = new Painter(mainpanel.CreateGraphics());
            p.Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            p.AddNew();
        }

        // Событие, которое происходит при изменнении размера панели
        private void mainpanel_Resize(object sender, EventArgs e)
        {
            // Изменяем размер панели
            // p.ContainerSize = mainpanel.Size;
            // Изменяем размер рисовашки и в нем меняется размер панели
            p.MainGraphics = mainpanel.CreateGraphics();
        }
    }
}