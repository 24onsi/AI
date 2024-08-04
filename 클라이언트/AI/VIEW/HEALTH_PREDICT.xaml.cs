using LiveCharts.Wpf;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
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
using LiveCharts.Definitions.Charts;

namespace AI.VIEW
{
    public partial class HEALTH_PREDICT : Page
    {
        public HEALTH_PREDICT()
        {
            InitializeComponent();
            init_Predict_UserValue();
            LoadChart();
        }

        private void btnp_Main_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/VIEW/MAIN.xaml", UriKind.Relative);
            NavigationService.Navigate(uri);
        }

        public void init_Predict_UserValue()
        {
            lbp_Name.Content = MAIN.mainData.User[0].Name;
            lbp_Age.Content = MAIN.mainData.Age;

            if (MAIN.mainData.User[0].Gender == 2)
            {
                lbp_Gender.Content = "여성";
            }
            else
            {
                lbp_Gender.Content = "남성";
            }

            lbp_ihdResult.Content = Convert.ToString(MAIN.mainData.PredictValue[0].IHD);
            lbp_stkResult.Content = Convert.ToString(MAIN.mainData.PredictValue[0].STK);
            lbp_htnResult.Content = Convert.ToString(MAIN.mainData.PredictValue[0].HTN);
            lbp_dmResult.Content = Convert.ToString(MAIN.mainData.PredictValue[0].DM);
        }

        public void LoadChart()
        {
            int ihd = MAIN.mainData.PredictValue[0].IHD;
            int stk = MAIN.mainData.PredictValue[0].STK;
            int htn = MAIN.mainData.PredictValue[0].HTN;
            int dm = MAIN.mainData.PredictValue[0].DM;

            var seriesCollection = new SeriesCollection
            {
                new RowSeries
                {
                    Values = new ChartValues<double> { dm, htn, stk, ihd },
                    Fill = new SolidColorBrush(Colors.Khaki)
                }
            };

            PredictChart.Series = seriesCollection;

            PredictChart.AxisX[0].MinValue = 0;
            PredictChart.AxisX[0].MaxValue = 100;
        }
    }
}
