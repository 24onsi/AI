#ifndef HANDLER_H
#define HANDLER_H

#include <iostream>
#include <cstdlib>
#include <cstring>    
#include <mariadb/conncpp.hpp> 
#include <nlohmann/json.hpp> 
#include <unistd.h>
#include <arpa/inet.h>
#include "DATA.h"

using json = nlohmann::json;


class DB
{
public:
    DB();
    ~DB();
    sql::Connection* Connect();
    void Disconnect(sql::Connection* conn);
};


class Handler
{
public:
    Handler();
    ~Handler();

    void ID_Check(const Data & data, int sock);
    void Login(const Data & data, int sock);
    void Join(const Data & data, int sock);
    void RangeNum(sql::Connection*con, std::string table, std::string column);
    void InsertDB_CheckItem(const Data & data);
    void CheckItem_Value(Data & data, int sock);
    void Load_History(const Data & data, int sock);
    void Search_Result(const Data & data, int sock);


private:

};

class AI
{
public:
    AI();
    ~AI();

    int ConnectPython();
    RESULT Predict_Value(Data & data);
    void InsertDB_PredictValue(const Data & data);

private:

};

#endif