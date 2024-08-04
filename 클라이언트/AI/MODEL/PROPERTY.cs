using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AI.VIEW;

namespace AI.MODEL
{
    public class ComboBoxList
    {
        public List<string> GenderList { get; set; } = new List<string>()
        {
            "남성",
            "여성"
        };
    }

    public class Check
    {
        public bool ID_Check { get; set; }
        public bool Login_Check { get; set; }
    }

    public class Data
    {
        public int KEY { get; set; }
        public int Type { get; set; }

        public string? ID { get; set; }
        public string? Password { get; set; }
        public int Age { get; set; }

        public List<USER> User { get; set; }
        public List<CHECK_ITEM> CheckItem { get; set; }
        public List<RESULT> PredictValue { get; set; }
    }

    public class USER
    {
        public string? Name { get; set; }
        public string? Birth { get; set; }
        public int Gender  { get; set; }
        public string? Phonenum { get; set; }
    }

    public class CHECK_ITEM
    { 
        public string? CheckDate { get; set; }
        public int Smoke { get; set; }
        public bool Drink { get; set; }
        public int SBP { get; set; }
        public int DBP { get; set; }
        public int GLU { get; set; }
        public int TC { get; set; }
        public int HDL { get; set; }
        public int TG { get; set; }
        public int BMI { get; set; }
    }

    public class RESULT
    {
        public int IHD { get; set; }
        public int STK { get; set; }
        public int HTN { get; set; }
        public int DM { get; set; }
    }

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

    }

}
