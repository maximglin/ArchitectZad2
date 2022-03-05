using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GrpcClient;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //this.DataContext = new CarsVM(
            //    this.Resources["CarsRepo"] as CarsRepository, 
            //    this.Resources["ColorsRepo"] as ColorsRepository,
            //    this.Resources["ManufacturersRepo"] as ManufacturersRepository
            //   );
        }


        private void DataGrid_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {
            //if (e.NewItem != null)
             //   (e.NewItem as CarVM).repo = this.Resources["CarsRepo"] as CarsRepository;
        }

        private void DataGrid_InitializingNewItem_1(object sender, InitializingNewItemEventArgs e)
        {
            //if (e.NewItem != null)
            //    (e.NewItem as DescriptionVMBase).repo = this.Resources["ManufacturersRepo"] as ManufacturersRepository;
        }

        private void DataGrid_InitializingNewItem_2(object sender, InitializingNewItemEventArgs e)
        {
            //if (e.NewItem != null)
            //    (e.NewItem as DescriptionVMBase).repo = this.Resources["ColorsRepo"] as ColorsRepository;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            //(this.Resources["CarsRepo"] as CarsRepository                  ).Dispose();
            //(this.Resources["ColorsRepo"] as ColorsRepository              ).Dispose();
            //(this.Resources["ManufacturersRepo"] as ManufacturersRepository).Dispose();
        }
    }
}
