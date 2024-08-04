#ifndef DATA_H
#define DATA_H
#include <string>
#include <vector>

struct USER
{
    std::string Name;       // 이름
    std::string Birth;      // 생년월일(8자리)
    int Gender;             // 성별
    std::string PhoneNum;   // 전화번호
};

struct CHECK_ITEM
{
    std::string Date;       // 날짜
    int Smoke;              // 흡연 여부
    bool Drink;             // 음주 여부
    int SBP;                // 수축기 혈압
    int DBP;                // 이완기 혈압
    int GLU;                // 공복 혈당
    int TC;                 // 총 콜레스테롤
    int HDL;                // 고밀도 콜레스테롤
    int TG;                 // 중성지방
    int BMI;                // 비만도

};

struct RESULT
{
    int IHD;              // 뇌.심혈관
    int STK;
    int HTN;                // 고혈압
    int DM;                 // 당뇨
};

struct Data
{
    int Type;
    int Age;
    std::string ID;
    std::string Password;

    USER User;
    CHECK_ITEM Check_Item;
    RESULT Result;
};

enum TYPE
{
    // 0번
    // 연결 실패
    CONNECT_FAIL = 0,

    // 10번
    // 아이디 확인
    ID_CHECK = 10,

    // 로그인
    LOGIN,
    
    // 회원가입
    JOIN,

    // 검진 데이터
    CHECK_UP,

    // 기록 요청
    HISTORY,

    // 기록 검색 결과
    SEARCH,

    // 통계
    STATISTIC,

    // 20번
    // 성공
    SUCCEED = 20,

    // 실패
    FAIL,

    // 오류
    ERROR,

};

#endif
