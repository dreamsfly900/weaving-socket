using GameDataAccess.BaseDAL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameDataAccess.CurrentDAL
{
    public class GameScoreTabelBaseDAL : Respository<GameScoreTabel>
    {


        public new bool Insert(GameScoreTabel newscore)
        {

            List<GameScoreTabel> allItem = base.GetALLEntity().ToList();
            if (allItem.Count == 0)
            {
                return base.Insert(newscore);
            }
            else
            {
                //判定所有元素里面有没有这个元素。。。
                bool itemInList =  allItem.Where(s=>s.userId == newscore.userId).FirstOrDefault() != null ? true : false;

                if (itemInList)
                {
                    return false;
                }
                else
                {
                    return base.Insert(newscore);
                }

            }

        }



        public new bool Update(GameScoreTabel newscore)
        {

            List<GameScoreTabel> allItem = base.GetALLEntity().ToList();
            allItem.Remove(allItem.Find(ns => ns.userId == newscore.userId));
            //判定再判定剩下的所有元素里面有没有这个元素。。。
            bool itemInList = allItem.Where(s => s.userId == newscore.userId).FirstOrDefault() != null ? true : false;
            if (itemInList)
            {
                return false;
            }
            else
            {
                return base.Update(newscore);
            }



        }

    }
}
