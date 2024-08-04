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
using AI.MODEL;
using LiveCharts.Maps;

namespace AI.VIEW
{
    public partial class LOGIN : Page
    {
        public LOGIN()
        {
            InitializeComponent();
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            textBox.Clear();
        }

        private void Password_GotFocus(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            passwordBox.Clear();
        }

        private void btnl_Join_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("회원가입 페이지로 이동합니다.");

            Uri uri = new Uri("/VIEW/JOIN.xaml", UriKind.Relative);
            NavigationService.Navigate(uri);
        }

        private void btnl_Login_Click(object sender, RoutedEventArgs e)
        {
            MAIN.mainData.Type = (int)TYPE.LOGIN;
            MAIN.mainData.ID = txbl_ID.Text.ToString();
            MAIN.mainData.Password = txbl_PW.Password.ToString();

            int result = MAIN.mainNet.Login();

            if(result == (int)TYPE.SUCCEED)
            {
                MAIN.check.Login_Check = true;
                MessageBox.Show("로그인 되었습니다. 메인화면으로 이동합니다.");

                Uri uri = new Uri("/VIEW/MAIN.xaml", UriKind.Relative);
                NavigationService.Navigate(uri);
            }
            else if(result == (int)TYPE.FAIL)
            {
                MAIN.check.Login_Check = false;
                MessageBox.Show("로그인 정보가 일치하지 않습니다. 다시 입력해주세요.");

                txbl_ID.Clear();
                txbl_PW.Clear();
            }
            else
            {
                MAIN.check.Login_Check = false;
                MessageBox.Show("오류가 발생했습니다.\n죄송합니다. 다시 시도해주시길 바랍니다.");

                txbl_ID.Clear();
                txbl_PW.Clear();
            }
        }

        private void btnl_back_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/VIEW/MAIN.xaml", UriKind.Relative);
            NavigationService.Navigate(uri);
        }
    }
}
