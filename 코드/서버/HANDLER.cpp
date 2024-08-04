#include "HANDLER.h"

DB::DB() {}
DB::~DB() { }

sql::Connection* DB::Connect()
{
    try
    {
        sql::Driver* driver = sql::mariadb::get_driver_instance();
        sql::SQLString url = "jdbc:mariadb://10.10.21.114:3306/HEALTH";
        sql::Properties properties({{"user", "OPERATOR"}, {"password", "1234"}});
        std::cout << "DB 접속 성공" << std::endl;

        return driver->connect(url, properties);   
     }
    catch(sql::SQLException& e)
    {
        std::cerr << "DB 접속 실패: " << e.what() << std::endl;
        exit(1);
    }
}

void DB::Disconnect(sql::Connection* conn)
{
    if (!conn->isClosed())
    {
        conn->close();
        std::cout << "DB 접속 해제" << std::endl;
    }
}


Handler::Handler() { }
Handler::~Handler() { }

void Handler::ID_Check(const Data & data, int sock)
{
    int SendByte = 0;
    json SendJson;
    std::string SendStr;

    try
    {
        std::cout << "ID 중복 체크" << std::endl;

        DB db;
        sql::Connection*con = db.Connect();
        sql::PreparedStatement*ID_Check = con->prepareStatement("SELECT ID FROM USER WHERE ID = ?");

        ID_Check->setString(1, data.ID);

        sql::ResultSet*res = ID_Check->executeQuery();

        if(res->rowsCount())
        {
            std::cout << "FAIL" << std::endl;
            SendJson = json{{"Type", FAIL}};
        }
        else
        {
            std::cout << "SUCCEED" << std::endl;
            SendJson = json{{"Type", SUCCEED}};
        }

        db.Disconnect(con);

        SendStr = SendJson.dump();
        SendByte = write(sock, SendStr.c_str(), SendStr.length());
        std::cout << SendStr << " " << SendByte << std::endl;

    }
    catch(const sql::SQLException& e)
    {
        std::cerr << "DB 조회 실패 : " << e.what() << std::endl;

        SendJson = json{{"Type", ERROR}};
        SendStr = SendJson.dump();
        SendByte = write(sock, SendStr.c_str(), SendStr.length());

        std::cout << SendStr << " " << SendByte << std::endl;

    }
}

void Handler::Login(const Data & data, int sock)
{
    USER user;
    int SendByte = 0;
    json SendJson;
    std::string SendStr;

    try
    {
        std::cout << "Login 조회" << std::endl;

        DB db;
        sql::Connection*con = db.Connect();
        sql::PreparedStatement*login
        = con->prepareStatement("SELECT NAME, GENDER, BIRTH, PHONENUM FROM USER WHERE ID = ? AND PASSWORD = ?");

        login->setString(1, data.ID);
        login->setString(2, data.Password);

        sql::ResultSet*res = login->executeQuery();

        if(res->rowsCount())
        {
            SendJson = json{{"Type", SUCCEED},
                            {"User", json::array()}};
            while(res->next())
            {
                user.Name = res->getString(1);
                user.Gender = res->getInt(2);
                user.Birth = res->getString(3);
                user.PhoneNum = res->getString(4);
                
                SendJson["User"].push_back({{"Name", user.Name},
                                            {"Gender", user.Gender},
                                            {"Birth", user.Birth},
                                            {"Phonenum", user.PhoneNum}});
            }

            std::cout << "SUCCEED" << std::endl;
        }
        else
        {
            std::cout << "FAIL" << std::endl;

            SendJson = json{{"Type", FAIL}};
        }

        db.Disconnect(con);

        SendStr = SendJson.dump();
        SendByte = write(sock, SendStr.c_str(), SendStr.length());

        std::cout << SendStr << " " << SendByte << std::endl;


    }
    catch(const sql::SQLException& e)
    {
        std::cerr << "DB 조회 실패 : " << e.what() << std::endl;

        SendJson = json{{"Type", ERROR}};
        SendStr = SendJson.dump();
        SendByte = write(sock, SendStr.c_str(), SendStr.length());

        std::cout << SendStr << " " << SendByte << std::endl;


    }
}

void Handler::Join(const Data & data, int sock)
{
    int SendByte = 0;
    json SendJson;
    std::string SendStr;

    try
    {
        std::cout << "Join 등록" << std::endl;

        DB db;
        sql::Connection*con = db.Connect();
        sql::PreparedStatement*join
        = con->prepareStatement("INSERT INTO USER VALUES(DEFAULT, ?, ?, ?, ?, ?, ?)");

        join->setString(1, data.ID);
        join->setString(2, data.Password);
        join->setString(3, data.User.Name);
        join->setString(4, data.User.Birth);
        join->setInt(5, data.User.Gender);
        join->setString(6, data.User.PhoneNum);

        join->executeQuery();

        RangeNum(con, "USER", "NUM");

        db.Disconnect(con);

        SendJson = json{{"Type", SUCCEED}};
        SendStr = SendJson.dump();
        SendByte = write(sock, SendStr.c_str(), SendStr.length());

        std::cout << "메세지 : " << SendStr << ", 바이트 : " << SendByte << std::endl;

    }
    catch(const sql::SQLException& e)
    {
        std::cerr << "DB 등록 실패 : " << e.what() << std::endl;

        SendJson = json{{"Type", ERROR}};
        SendStr = SendJson.dump();
        SendByte = write(sock, SendStr.c_str(), SendStr.length());

        std::cout << "메세지 : " << SendStr << ", 바이트 : " << SendByte << std::endl;
        
    }
}

void Handler::RangeNum(sql::Connection*con, std::string table, std::string column)
{
    sql::PreparedStatement*NUM1 = con->prepareStatement("ALTER TABLE " + table + " AUTO_INCREMENT=1");         
    NUM1->executeQuery();
    sql::PreparedStatement*NUM2 = con->prepareStatement("SET @COUNT = 0");
    NUM2->executeQuery();
    sql::PreparedStatement*NUM3 = con->prepareStatement("UPDATE " + table + " SET " + column + " = @COUNT:=@COUNT+1");
    NUM3->executeQuery();
}

void Handler::InsertDB_CheckItem(const Data & data)
{
    try
    {
        DB db;
        sql::Connection*con = db.Connect();
        sql::PreparedStatement*check
        = con->prepareStatement("INSERT INTO HISTORY VALUES(DEFAULT, ?, ?, DEFAULT, ?, ?, ?, ?, ?, ?, ?, ?, ?)");
        
        check->setString(1, data.ID);
        check->setInt(2, data.Age);
        check->setInt(3, data.Check_Item.Smoke);
        check->setBoolean(4, data.Check_Item.Drink);
        check->setInt(5, data.Check_Item.SBP);
        check->setInt(6, data.Check_Item.DBP);
        check->setInt(7, data.Check_Item.GLU);
        check->setInt(8, data.Check_Item.TC);
        check->setInt(9, data.Check_Item.HDL);
        check->setInt(10, data.Check_Item.TG);
        check->setInt(11, data.Check_Item.BMI);

        check->executeQuery();

        RangeNum(con, "HISTORY", "NO");


        std::cout << "검진 데이터 등록" << std::endl;


        db.Disconnect(con);
    }
    catch(const sql::SQLException& e)
    {
        std::cerr << "DB 등록 실패 : " << e.what() << std::endl;
    }
}

void Handler::CheckItem_Value(Data & data, int sock)
{
    int SendByte = 0;
    json SendJson;
    std::string SendStr;

    try
    {
        std::cout << "예측값 보내기" << std::endl;
        AI ai;

        // DB에 값 저장
        InsertDB_CheckItem(data);

        // 파이썬에서 예측값 가져오기
        data.Result = ai.Predict_Value(data);

        // C# 예측값 보내기
        SendJson = json{{"Type", SUCCEED},
                        {"PredictValue", json::array()}};

        SendJson["PredictValue"].push_back({{"IHD", data.Result.IHD},
                                            {"STK", data.Result.STK},        
                                            {"HTN", data.Result.HTN},
                                            {"DM", data.Result.DM}});

        SendStr = SendJson.dump();
        SendByte = write(sock, SendStr.c_str(), SendStr.length());

        std::cout << "메세지 : " << SendStr << ", 바이트 : " << SendByte << std::endl;

    }
    catch(const std::exception& e)
    {
        std::cerr << "오류 : " << e.what() << std::endl;
    }
}

void Handler::Load_History(const Data & data, int sock)
{
    int SendByte = 0;
    json SendJson;
    std::string SendStr;

    try
    {
        std::cout << "기록 불러오기" << std::endl;

        DB db;
        sql::Connection*con = db.Connect();
        sql::PreparedStatement*history
        = con->prepareStatement("SELECT * FROM HISTORY WHERE USER_ID = ?");

        history->setString(1, data.ID);

        sql::ResultSet*res = history->executeQuery();

        if(res->rowsCount())
        {
            SendJson = json{{"Type", SUCCEED},
                            {"CheckItem", json::array()}};

            while(res->next())
            {
                SendJson["CheckItem"].push_back({{"CheckDate", res->getString(4)},
                                                {"Smoke", res->getInt(5)},
                                                {"Drink", res->getBoolean(6)},
                                                {"SBP", res->getInt(7)},
                                                {"DBP", res->getInt(8)},
                                                {"GLU", res->getInt(9)},
                                                {"TC", res->getInt(10)},
                                                {"HDL", res->getInt(11)},
                                                {"TG", res->getInt(12)},
                                                {"BMI", res->getInt(13)}});
            }
        }
        else
        {
            SendJson = json{{"Type", FAIL}};
        }

        db.Disconnect(con);

        SendStr = SendJson.dump();
        SendByte = write(sock, SendStr.c_str(), SendStr.length());

        std::cout << "메세지 : " << SendStr << ", 바이트 : " << SendByte << std::endl;
    }
    catch(const sql::SQLException& e)
    {
        std::cerr << "DB 조회 실패 : " << e.what() << std::endl;

        SendJson = json{{"Type", ERROR}};
        SendStr = SendJson.dump();
        SendByte = write(sock, SendStr.c_str(), SendStr.length());

        std::cout << "메세지 : " << SendStr << ", 바이트 : " << SendByte << std::endl;
    }
}

void Handler::Search_Result(const Data & data, int sock)
{
    int SendByte = 0;
    json SendJson;
    std::string SendStr;

    try
    {
        std::cout << "결과 검색" << std::endl;

        DB db;
        sql::Connection*con = db.Connect();
        sql::PreparedStatement*result
        = con->prepareStatement("SELECT * FROM RESULT WHERE RE_ID = ? AND RE_DATE = ?");

        result->setString(1, data.ID);
        result->setString(2, data.Check_Item.Date);

        sql::ResultSet*res = result->executeQuery();

        if(res->rowsCount())
        {
            SendJson = json{{"Type", SUCCEED},
                            {"PredictValue", json::array()}};

            while(res->next())
            {
                SendJson["PredictValue"].push_back({{"IHD", res->getInt(3)},
                                                    {"STK", res->getInt(4)},
                                                    {"HTN", res->getInt(5)},
                                                    {"DM", res->getInt(6)}});
                
                break;
            }
        }
        else
        {
            SendJson = json{{"Type", FAIL}};
        }

        db.Disconnect(con);

        SendStr = SendJson.dump();
        SendByte = write(sock, SendStr.c_str(), SendStr.length());

        std::cout << "메세지 : " << SendStr << ", 바이트 : " << SendByte << std::endl;
    }
    catch(const sql::SQLException& e)
    {
        std::cerr << "DB 조회 실패 : " << e.what() << std::endl;

        SendJson = json{{"Type", ERROR}};
        SendStr = SendJson.dump();
        SendByte = write(sock, SendStr.c_str(), SendStr.length());

        std::cout << "메세지 : " << SendStr << ", 바이트 : " << SendByte << std::endl;
    }
}


AI::AI() { }
AI::~AI() { }

int AI::ConnectPython()
{

    int Port = 5000;
    const char IP[] = "10.10.21.114";
    struct sockaddr_in py_adr;

    int py_sock = socket(PF_INET, SOCK_STREAM, 0);

    memset(&py_adr, 0, sizeof(py_adr));

    py_adr.sin_family = AF_INET;
    py_adr.sin_addr.s_addr = inet_addr(IP);
    py_adr.sin_port = htons(Port);
    inet_pton(AF_INET, IP, &py_adr.sin_addr);

    if(connect(py_sock,(struct sockaddr*)&py_adr, sizeof(py_adr)) < 0)
    {
        std::cout << "파이썬 소켓 생성 실패" << std::endl;
    }

    std::cout << "파이썬 서버 연결" << std::endl;

    // 데이터 확인 ( 타입 )
    std::cout << "py_sock: " << py_sock << std::endl;

    return py_sock;

}

void AI::InsertDB_PredictValue(const Data & data)
{
    try
    {
        DB db;
        sql::Connection*con = db.Connect();
        sql::PreparedStatement*result
        = con->prepareStatement("INSERT INTO RESULT VALUES(?, DEFAULT, ?, ?, ?, ?)");
        
        result->setString(1, data.ID);
        result->setInt(2, data.Result.IHD);
        result->setInt(3, data.Result.STK);
        result->setInt(4, data.Result.HTN);
        result->setInt(5, data.Result.DM);

        result->executeQuery();

        db.Disconnect(con);

    }
    catch(const sql::SQLException& e)
    {
        std::cerr << "DB 저장 실패 : " << e.what() << std::endl;
    }
}

RESULT AI::Predict_Value(Data & data)
{
    int SendByte, ReadByte = 0;
    json SendJson, ReadJson;
    std::string SendStr;
    char buffer[1024] = {0};

    int sock = ConnectPython();

    SendJson = json{{"Gender", data.User.Gender}, {"Age", data.Age}, {"Smoke", data.Check_Item.Smoke},
                    {"Drink", data.Check_Item.Drink}, {"SBP", data.Check_Item.SBP}, {"DBP", data.Check_Item.DBP},
                    {"GLU", data.Check_Item.GLU}, {"TC", data.Check_Item.TC}, {"HDL", data.Check_Item.HDL},
                    {"TG", data.Check_Item.TG}, {"BMI", data.Check_Item.BMI}};

    SendStr = SendJson.dump();
    SendByte = write(sock, SendStr.c_str(), SendStr.length());

    std::cout << "메세지 : " << SendStr << ", 바이트 : " << SendByte << std::endl;

    // 데이터를 받는다
    ReadByte = recv(sock, buffer, 1024, 0);
    std::string temp(buffer, ReadByte);
    ReadJson = json::parse(temp);

    std::cout << "받은 메세지 : " << ReadJson << std::endl;

    data.Result.IHD = ReadJson.value("IHD", 0.0);
    data.Result.STK = ReadJson.value("STK", 0.0);
    data.Result.HTN = ReadJson.value("HTN", 0);
    data.Result.DM = ReadJson.value("DM", 0);

    //DB 값 저장
    InsertDB_PredictValue(data);

    close(sock);

    return data.Result;

}



