using GameDataAccess.CurrentDAL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameDataAccess.BLL
{
     public class GameScoreTabelBLL
    {

        public bool UpdateGameScore(string _uname, int _score , int _missed)
        {
            bool updateResult = false;

            UserTabelBaseDAL uDAL = new UserTabelBaseDAL();
            UserTabel user = new UserTabel();
            user = uDAL.GetOneByWhere(u => u.userName == _uname);
            if (user != null) {

                GameScoreTabelBaseDAL gsDAL = new GameScoreTabelBaseDAL();
                GameScoreTabel gsModel = new GameScoreTabel();



                gsModel = gsDAL.GetOneByWhere(s => s.userId == user.Id);

                gsModel.score = _score;
                gsModel.missenemy = _missed;

                updateResult = gsDAL.Update(gsModel);

                return updateResult;
            }
            else
            {
                return updateResult;
            }
           
        }


        public GameScoreTabel GetGameScore(string _uname)
        {
            // bool updateResult = false;
            GameScoreTabel gsModel = new GameScoreTabel();


            UserTabelBaseDAL uDAL = new UserTabelBaseDAL();
            UserTabel user = new UserTabel();
            user = uDAL.GetOneByWhere(u => u.userName == _uname);
            if (user != null)
            {

                GameScoreTabelBaseDAL gsDAL = new GameScoreTabelBaseDAL();
               // GameScoreTabel gsModel = new GameScoreTabel();



                gsModel = gsDAL.GetOneByWhere(s => s.userId == user.Id);

              

                return gsModel;
            }
            else
            {
                return null;
            }

        }


        public bool CheckDataBaseIsNull()
        {
            bool databaseIsNull = false;

            UserTabelBaseDAL uDAL = new UserTabelBaseDAL();

            databaseIsNull = uDAL.GetALLEntity().Count == 0 ? true : false;

            return databaseIsNull;
        }

    }
}
