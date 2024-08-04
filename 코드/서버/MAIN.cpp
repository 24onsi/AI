#include "HANDLER.cpp"

using json = nlohmann::json;

#define PORT_NUM 5001

void error_handling(const char *msg);
void *handle_clnt(void *arg);

int main()
{
    int serv_sock;
    struct sockaddr_in serv_adr;

    serv_sock = socket(PF_INET, SOCK_STREAM, 0);

    if (serv_sock == -1)
    {
        error_handling("서버 소켓 생성 실패");
    }

    memset(&serv_adr, 0, sizeof(serv_adr));

    serv_adr.sin_family = AF_INET;
    serv_adr.sin_addr.s_addr = INADDR_ANY;
    serv_adr.sin_port = htons(PORT_NUM);


    if (bind(serv_sock, (struct sockaddr *)&serv_adr, sizeof(serv_adr)) < 0)
    {
        error_handling("바인딩 실패");
    }

    if (listen(serv_sock, 5) == -1)
    {
        error_handling("소켓 리슨 실패");
    }

    while (1)
    {
        int clnt_sock;
        struct sockaddr_in clnt_adr;
        socklen_t clnt_adr_sz = sizeof(clnt_adr_sz);

        clnt_sock = accept(serv_sock, (struct sockaddr *)&clnt_adr, &clnt_adr_sz);

        if (clnt_sock == -1)
        {
            std::cerr << "클라이언트 연결 수락 실패";
            close(clnt_sock);
        }

        pthread_t clnt_thread;
        pthread_create(&clnt_thread, nullptr, handle_clnt, (void *)&clnt_sock);
        pthread_detach(clnt_thread);
    }


    close(serv_sock);

    return 0;
}


void *handle_clnt(void *arg)
{
    int clnt_sock = *((int*)arg);
    char buffer[1024] = {0};

    try
    {
        Data ReadData;

        int read = recv(clnt_sock, buffer, 1024, 0);

        std::string temp(buffer, read);
        json ReadJson = json::parse(temp);
        json ListJson;

        std::cout << ReadJson << std::endl;

        ReadData.Type = ReadJson.value("Type", 0);
        ReadData.ID = ReadJson.value("ID", "");

        std::cout << "sock: " << clnt_sock << " Type: " << ReadData.Type << " ID: " << ReadData.ID << std::endl;

        Client clnt;

        switch (ReadData.Type)
        {
            case ID_CHECK:
                std::cout << "ID_CHECK" << std::endl;

                clnt.ID_Check(ReadData, clnt_sock);

                break;

            case LOGIN:
                std::cout << "LOGIN" << std::endl;

                ReadData.Password = ReadJson.value("Password", "");

                clnt.Login(ReadData, clnt_sock);

                break;

            case JOIN:
                std::cout << "JOIN" << std::endl;

                ReadData.Password = ReadJson.value("Password", "");
                ListJson = ReadJson.value("User", json::array());
                ReadData.User.Name = ListJson[0].value("Name", "");
                ReadData.User.Birth = ListJson[0].value("Birth", "");
                ReadData.User.Gender = ListJson[0].value("Gender", 1);
                ReadData.User.PhoneNum = ListJson[0].value("Phonenum", "");

                clnt.Join(ReadData, clnt_sock);

                break;

            case CHECK_UP:
                std::cout << "CHECK_UP" << std::endl;

                ReadData.Age = ReadJson.value("Age", 0);
                ReadData.User.Gender = ReadJson.value("KEY", 1); 
                ListJson = ReadJson.value("CheckItem", json::array());
                ReadData.Check_Item.Smoke = ListJson[0].value("Smoke", 0);
                ReadData.Check_Item.Drink = ListJson[0].value("Drink", true);
                ReadData.Check_Item.SBP = ListJson[0].value("SBP", 0);
                ReadData.Check_Item.DBP = ListJson[0].value("DBP", 0);
                ReadData.Check_Item.GLU = ListJson[0].value("GLU", 0);
                ReadData.Check_Item.TC = ListJson[0].value("TC", 0);
                ReadData.Check_Item.HDL = ListJson[0].value("HDL", 0);
                ReadData.Check_Item.TG = ListJson[0].value("TG", 0);
                ReadData.Check_Item.BMI = ListJson[0].value("BMI", 0);

                clnt.CheckItem_Value(ReadData, clnt_sock);

                break;

            case HISTORY:
                std::cout << "HISTORY" << std::endl;

                clnt.Load_History(ReadData, clnt_sock);

                break;

            case SEARCH:
                std::cout << "SEARCH" << std::endl;

                ListJson = ReadJson.value("CheckItem", json::array());
                ReadData.Check_Item.Date = ListJson[0].value("CheckDate", "");

                clnt.Search_Result(ReadData, clnt_sock);

                break;

            default:
                break;
        }
    }
    
    catch(const std::exception& e)
    {
        std::cerr << e.what() << '\n';
        try
        {
            json js = json{{"Type", CONNECT_FAIL}};
            std::string sendData = js.dump();
            write(clnt_sock, sendData.c_str(), sendData.length());
        }
        catch(const std::exception& e)
        {
            std::cerr << e.what() << '\n';
        }
    }

    std::cout << clnt_sock << " : 접속종료" << std::endl;
    close(clnt_sock);
    return nullptr;
}

void error_handling(const char *message)
{
    fputs(message, stderr);
    fputc('\n', stderr);
    exit(1);
}

