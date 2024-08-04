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

namespace AI.VIEW
{
    public partial class JOIN : Page
    {
        public JOIN()
        {
            InitializeComponent();

            ComboBoxList cb = new ComboBoxList();
            cb_Gender.ItemsSource = cb.GenderList;
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

        public void UserList_value()
        {
            USER user = new USER();
            List<USER> ListUser = new List<USER>();

            MAIN.mainData.Type = (int)TYPE.JOIN;
            MAIN.mainData.ID = txbj_ID.Text.ToString();
            MAIN.mainData.Password = txbj_PW1.Password.ToString();

            user.Name = txbj_Name.Text.ToString();
            user.Birth = txbj_Birth.Text.ToString();
            user.Phonenum = txbj_Phone.Text.ToString();

            int n = cb_Gender.SelectedIndex;

            int gender = 1;

            if (n == 1) { gender = 2; }

            user.Gender = gender;

            ListUser.Add(user);
            MAIN.mainData.User = ListUser;
        }

        private void btnj_IDcheck_Click(object sender, RoutedEventArgs e)
        {
            int result;

            if (!string.IsNullOrWhiteSpace(txbj_ID.Text))
            {
                MAIN.mainData.ID = txbj_ID.Text.ToString();
                MAIN.mainData.Type = (int)TYPE.ID_CHECK;

                result = MAIN.mainNet.ID_Matching();
                if (result == (int)TYPE.SUCCEED)
                {
                    MAIN.check.ID_Check = true;
                    MessageBox.Show("사용가능한 아이디 입니다.");
                }
                else if (result == (int)TYPE.FAIL)
                {
                    MAIN.check.ID_Check = false;
                    txbj_ID.Clear();
                    MessageBox.Show("이미 사용중인 아이디 입니다.");
                }
                else
                {
                    MAIN.check.ID_Check = false;
                    MessageBox.Show("오류가 발생했습니다.\n죄송합니다. 다시 확인해주시길 바랍니다.");
                }
            }
        }

        private void btnj_back_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/VIEW/MAIN.xaml", UriKind.Relative);
            NavigationService.Navigate(uri);
        }

        private void btnj_Join_Click(object sender, RoutedEventArgs e)
        {
            int result;

            if (MAIN.check.ID_Check)
            {
                if (!string.IsNullOrWhiteSpace(txbj_ID.Text) && !string.IsNullOrWhiteSpace(txbj_PW1.Password)
                    && !string.IsNullOrWhiteSpace(txbj_PW2.Password) && !string.IsNullOrWhiteSpace(txbj_Name.Text)
                    && !string.IsNullOrWhiteSpace(txbj_Birth.Text) && !string.IsNullOrWhiteSpace(txbj_Phone.Text))
                {

                    UserList_value();

                    result = MAIN.mainNet.Join();

                    if (result == (int)TYPE.SUCCEED)
                    {
                        MessageBox.Show("회원가입 되었습니다.\n로그인 페이지로 이동합니다.");

                        Uri uri = new Uri("/VIEW/LOGIN.xaml", UriKind.Relative);
                        NavigationService.Navigate(uri);
                    }
                    else
                    {
                        MessageBox.Show("회원가입에 실패했습니다. 다시 시도해주시길 바랍니다.");
                    }
                }
                else
                {
                    MessageBox.Show("공백없이 모두 입력해주세요.");
                }
            }
            else
            {
                MessageBox.Show("아이디 중복확인 해주세요.");
            }
        }
    }
}
