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
    public partial class MAIN : Page
    {
        public MAIN()
        {
            InitializeComponent();

            init_MainLabel();
        }

        public static Data mainData = new Data();
        public static PROCESS mainNet = new PROCESS();
        public static Check check = new Check();
        
        private void btnm_Health_Click(object sender, RoutedEventArgs e)
        {
            if (check.Login_Check == true)
            {
                Uri uri = new Uri("/VIEW/HEALTH_DATA.xaml", UriKind.Relative);
                NavigationService.Navigate(uri);
            }
            else
            {
                MessageBox.Show("로그인이 필요한 서비스입니다.\n로그인 해주시길 바랍니다.");
            }
        }

        private void btnm_Login_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/VIEW/LOGIN.xaml", UriKind.Relative);
            NavigationService.Navigate(uri);
        }

        private void btnm_MyPage_Click(object sender, RoutedEventArgs e)
        {
            if(check.Login_Check == true)
            {
                Uri uri = new Uri("/VIEW/MY_PAGE.xaml", UriKind.Relative);
                NavigationService.Navigate(uri);
            }
            else
            {
                MessageBox.Show("로그인이 필요한 서비스입니다.\n로그인 해주시길 바랍니다.");
            }
        }

        public void init_MainLabel()
        {
            if(check.Login_Check)
            {
                lb_mainName.Content = mainData.ID + "님 안녕하세요. 반갑습니다.";
            }
        }

        private void btnm_Logout_Click(object sender, RoutedEventArgs e)
        {
            if (check.Login_Check)
            {
                if (MessageBox.Show("로그아웃 하시겠습니까?", "로그아웃", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    check.Login_Check = false;
                    lb_mainName.Content = "안녕하세요. 반갑습니다.";
                }
            }
            else
            {
                MessageBox.Show("현재 로그인 상태가 아닙니다.");
            }
        }
    }
}
