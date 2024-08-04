using AI.MODEL;
using LiveCharts.Maps;
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

namespace AI.VIEW
{

    public partial class HEALTH_DATA : Page
    {
        public HEALTH_DATA()
        {
            InitializeComponent();
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            textBox.Clear();
        }

        private void btnd_Predict_Click(object sender, RoutedEventArgs e)
        {
            int result;

            if (!string.IsNullOrWhiteSpace(txbd_Height.Text) && !string.IsNullOrWhiteSpace(txbd_Weight.Text)
                && !string.IsNullOrWhiteSpace(txbd_Glu.Text) && !string.IsNullOrWhiteSpace(txbd_SBP.Text)
                && !string.IsNullOrWhiteSpace(txbd_DBP.Text) && !string.IsNullOrWhiteSpace(txbd_TC.Text)
                && !string.IsNullOrWhiteSpace(txbd_TG.Text) && !string.IsNullOrWhiteSpace(txbd_HDL.Text))
            {
                checkItem_value();

                result = MAIN.mainNet.Check_Item();

                if(result == (int)TYPE.SUCCEED)
                {
                    Uri uri = new Uri("/VIEW/HEALTH_PREDICT.xaml", UriKind.Relative);
                    NavigationService.Navigate(uri);
                }
                else
                {
                    MessageBox.Show("오류가 발생하였습니다. 다시 버튼을 눌러주시길 바랍니다.");
                }
            }
        }

        public void checkItem_value()
        {
            CHECK_ITEM checkItem = new CHECK_ITEM();
            List<CHECK_ITEM> List_checkItem = new List<CHECK_ITEM>();

            MAIN.mainData.Type = (int)TYPE.CHECK_UP;

            float Height = float.Parse(txbd_Height.Text);
            float Weight = float.Parse(txbd_Weight.Text);
            checkItem.BMI = MAIN.mainNet.BMI(Height, Weight);

            checkItem.CheckDate = MAIN.mainNet.Today();
            checkItem.SBP = int.Parse(txbd_SBP.Text);
            checkItem.DBP = int.Parse(txbd_DBP.Text);
            checkItem.GLU = int.Parse(txbd_Glu.Text);
            checkItem.TC = int.Parse(txbd_TC.Text);
            checkItem.HDL = int.Parse(txbd_HDL.Text);
            checkItem.TG = int.Parse(txbd_TG.Text);

            if (rb_NoSmoke.IsChecked == true)
            {
                checkItem.Smoke = 1;
            }
            else if(rb_Smoke.IsChecked == true)
            {
                checkItem.Smoke = 2;
            }
            else if (rb_StopSmoke.IsChecked == true)
            {
                checkItem.Smoke = 3;
            }

            if (rb_NoDrink.IsChecked == true)
            {
                checkItem.Drink = false;
            }
            else if (rb_Drink.IsChecked == true)
            {
                checkItem.Drink = true;
            }

            List_checkItem.Add(checkItem);

            MAIN.mainData.CheckItem = List_checkItem;
        }

        private void btnd_back_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/VIEW/MAIN.xaml", UriKind.Relative);
            NavigationService.Navigate(uri);
        }
    }
}
