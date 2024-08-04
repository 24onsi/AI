using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Net.Sockets;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.IO;
using System.Diagnostics;
using System.Net;
using AI.VIEW;
using System.DirectoryServices.ActiveDirectory;

namespace AI.MODEL
{
    public class PROCESS
    {
        public PROCESS() { }

        private TcpClient Clnt;
        private NetworkStream Stream;

        private string IP = "10.10.21.118";
        private int Port = 5001;

        private int Connect()
        {
            try
            {
                Clnt = new TcpClient(IP, Port);
                Stream = Clnt.GetStream();
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return 0;
            }

            return 1;
        }

        private void Disconnect()
        {
            Stream.Close();
            Clnt.Close();
        }

        public void SendMsg(Data msgdata)
        {
            string Sendmsg = JsonSerializer.Serialize(msgdata);
            byte[] SendByte = Encoding.UTF8.GetBytes(Sendmsg);
            Stream.Write(SendByte, 0, SendByte.Length);
        }

        public string ReadMsg()
        {
            byte[] Buffer = new byte[1024];
            int ReadByte = Stream.Read(Buffer, 0, Buffer.Length);
            string ReadJson = Encoding.UTF8.GetString(Buffer, 0, ReadByte);

            return ReadJson;
        }

        public int Login()
        {
            if (Connect() == 0)
            {
                return 0;
            }

            Data sendData = new Data()
            {
                Type = MAIN.mainData.Type,
                ID = MAIN.mainData.ID,
                Password = MAIN.mainData.Password,
            };

            SendMsg(sendData);

            string Readmsg = ReadMsg();

            Disconnect();

            try
            {
                Data? Result = JsonSerializer.Deserialize<Data>(Readmsg);
                
                if(Result.Type == (int)TYPE.SUCCEED)
                {
                    MAIN.mainData.User = Result.User;
                    SetAge();
                }

                return Result.Type;
            }
            catch (JsonException jsonEx)
            {
                Debug.WriteLine(jsonEx.Message);
                return 22;
            }
        }

        public void SetAge()
        {
            DateTime birthDate = DateTime.Parse(MAIN.mainData.User[0].Birth);
            DateTime Today = DateTime.Today;

            int age = Today.Year - birthDate.Year;

            if (birthDate.Date > Today.AddYears(-age))
            {
                age -= 1;
            }

            MAIN.mainData.Age = age;
        }

        public int ID_Matching()
        {
            if (Connect() == 0)
            {
                return 0;
            }

            Data sendData = new Data()
            {
                Type = MAIN.mainData.Type,
                ID = MAIN.mainData.ID,
            };

            SendMsg(sendData);

            string Readmsg = ReadMsg();

            Disconnect();

            try
            {
                Data? Result = JsonSerializer.Deserialize<Data>(Readmsg);


                return Result.Type;
            }
            catch (JsonException jsonEx)
            {
                Debug.WriteLine(jsonEx.Message);
                return 22;
            }
        }

        public int Join()
        {
            if (Connect() == 0)
            {
                return 0;
            }

            Data sendData = new Data()
            {
                Type = MAIN.mainData.Type,
                ID = MAIN.mainData.ID,
                Password = MAIN.mainData.Password,
                User = MAIN.mainData.User,
            };

            SendMsg(sendData);

            string Readmsg = ReadMsg();

            Debug.WriteLine(Readmsg);

            Disconnect();

            try
            {

                Data? Result = JsonSerializer.Deserialize<Data>(Readmsg);

                return Result.Type;
            }
            catch (JsonException jsonEx)
            {
                Debug.WriteLine(jsonEx.Message);
                return 22;
            }
        }

        public int Check_Item()
        {
            if (Connect() == 0)
            {
                return 0;
            }

            Data sendData = new Data()
            {
                Type = MAIN.mainData.Type,
                ID = MAIN.mainData.ID,
                Age = MAIN.mainData.Age,
                KEY = MAIN.mainData.User[0].Gender,
                CheckItem = MAIN.mainData.CheckItem
            };

            SendMsg(sendData);

            string Readmsg = ReadMsg();

            Disconnect();

            try
            {
                Data? Result = JsonSerializer.Deserialize<Data>(Readmsg);

                if(Result.Type == (int)TYPE.SUCCEED)
                {
                    MAIN.mainData.PredictValue = Result.PredictValue;
                }

                return Result.Type;
            }
            catch (JsonException jsonEx)
            {
                Debug.WriteLine(jsonEx.Message);
                return 22;
            }
        }

        public int BMI(float height, float weight)
        {
            int bmi;

            height /= 100;
            height *= height;
            bmi = (int)Math.Round(weight / height);

            return bmi;
        }

        public string Today()
        {
            DateTime today = DateTime.Now;
            string str_today = Convert.ToString(today);

            return str_today;
        }

        public int Load_History()
        {
            if (Connect() == 0)
            {
                return 0;
            }

            Data sendData = new Data()
            {
                Type = (int)TYPE.HISTORY,
                ID = MAIN.mainData.ID,
            };

            SendMsg(sendData);

            string Readmsg = ReadMsg();

            Disconnect();

            try
            {
                Data? Result = JsonSerializer.Deserialize<Data>(Readmsg);

                if (Result.Type == (int)TYPE.SUCCEED)
                {
                    MAIN.mainData.CheckItem = Result.CheckItem;
                }

                return Result.Type;
            }
            catch (JsonException jsonEx)
            {
                Debug.WriteLine(jsonEx.Message);
                return 22;
            }
        }
    }

}
