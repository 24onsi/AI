using AI.MODEL;
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
    public partial class MY_PAGE : Page
    {
        public static PROCESS MyPageNet = new PROCESS();

        public MY_PAGE()
        {
            InitializeComponent();
            init_MyPage_UserValue();
            init_HistoryList();
        }

        private void btnm_Main_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/VIEW/MAIN.xaml", UriKind.Relative);
            NavigationService.Navigate(uri);
        }

        public void init_MyPage_UserValue()
        {
            lbm_Name.Content = MAIN.mainData.User[0].Name;
            lbm_Age.Content = MAIN.mainData.Age;

            if (MAIN.mainData.User[0].Gender == 2)
            {
                lbm_Gender.Content = "여성";
            }
            else
            {
                lbm_Gender.Content = "남성";
            }
        }

        public void init_HistoryList()
        {
            int result;
            List<CHECK_ITEM> checkList = new List<CHECK_ITEM>();

            Task.Run(async () =>
            {
                checkList.Clear();
                result = MAIN.mainNet.Load_History();

                if (result == (int)TYPE.SUCCEED)
                {
                    await Dispatcher.BeginInvoke(new Action(() =>
                    {
                        foreach (var item in MAIN.mainData.CheckItem)
                        {
                            CHECK_ITEM checkitem = new CHECK_ITEM();

                            checkitem.CheckDate = item.CheckDate;
                            checkitem.BMI = item.BMI;
                            checkitem.SBP = item.SBP;
                            checkitem.DBP = item.DBP;
                            checkitem.TC = item.TC;
                            checkitem.TG = item.TG;
                            checkitem.HDL = item.HDL;
                            checkitem.GLU = item.GLU;

                            checkList.Add(checkitem);
                        }
                        lv_myHistory.ItemsSource = checkList;
                        lv_myHistory.Items.Refresh();
                    }));
                }
                else if (result == (int)TYPE.FAIL)
                {
                    await Dispatcher.BeginInvoke(new Action(() =>
                    {
                        MessageBox.Show("기록이 없습니다.\n메인화면으로 이동합니다.");
                        Uri uri = new Uri("/VIEW/MAIN.xaml", UriKind.Relative);
                        NavigationService.Navigate(uri);
                    }));
                }
                else if (result == (int)TYPE.ERROR)
                {
                    await Dispatcher.BeginInvoke(new Action(() =>
                    {
                        MessageBox.Show("기록 불러오기 실패하였습니다.\n메인화면으로 이동합니다.");
                        Uri uri = new Uri("/VIEW/MAIN.xaml", UriKind.Relative);
                        NavigationService.Navigate(uri);
                    }));
                }
            });
        }

        public void Select_HistoryLog(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            CHECK_ITEM checkItem = new CHECK_ITEM();
            List<CHECK_ITEM> checkList = new List<CHECK_ITEM>();

            int result;

            if (lv_myHistory.SelectedItem is CHECK_ITEM Log)
            {
                foreach (var item in MAIN.mainData.CheckItem)
                {
                    if (Log.CheckDate == item.CheckDate)
                    {
                        checkItem.CheckDate = item.CheckDate;
                        checkList.Add(item); 
                        MAIN.mainData.CheckItem = checkList;
                        MAIN.mainData.Type = (int)TYPE.SEARCH;

                        break;
                    }
                }

                result = MAIN.mainNet.Check_Item();

                if(result == (int)TYPE.SUCCEED)
                {
                    if (MessageBox.Show($"건강 위험도 분석 페이지로 이동하시겠습니까?", "페이지 이동", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        Uri uri = new Uri("/VIEW/HEALTH_PREDICT.xaml", UriKind.Relative);
                        NavigationService.Navigate(uri);
                    }
                }
                else
                {
                    MessageBox.Show("오류가 발생했습니다. 다시 시도해주세요.");
                }

            }
        }
    }
}
