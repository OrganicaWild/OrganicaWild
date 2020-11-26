using System;
using Framework.GraphGrammar;
using UnityEngine;

namespace Demo.GraphGrammar
{
    [Serializable]
    public class DDorman
    {
        [SerializeField] 
        public DDormanType type;

        public DDorman(DDormanType type)
        {
            this.type = type;
        }

        [Serializable]
        public enum DDormanType
        {
            BossLevel, //boss lever
            BossMini, //mini boss
            Chain, //chain
            ChainFinal, //chain final
            ChainLinear, //chain linear
            ChainParallel, //chain parallel
            Entrance, //entrance
            Fork, //fork
            Gate, //gate
            Goal, //goal
            Hook, //hook
            ItemBonus, //item bonus
            ItemQuest, //item quest
            Key, //key
            KeyFinal, //key (final)
            KeyMultiPiece, //key multi piece
            Lock, //lock
            LockFinal, //lock (final)
            LockMulti, //lock (multi)
            Nothing, //nothing, exploration
            Start, //Start
            Test, //test
            TestItem, //test (item)
            TestSecret //test (secret)
        }

        public bool IsTerminal()
        {
            switch (type)
            {
                case DDormanType.BossLevel:
                    return true;
                case DDormanType.BossMini:
                    return true;
                case DDormanType.Chain:
                    return false;
                case DDormanType.ChainFinal:
                    return false;
                case DDormanType.ChainLinear:
                    return false;
                case DDormanType.ChainParallel:
                    return false;
                case DDormanType.Entrance:
                    return true;
                case DDormanType.Fork:
                    return true;
                case DDormanType.Gate:
                    return false;
                case DDormanType.Goal:
                    return true;
                case DDormanType.Hook:
                    return false;
                case DDormanType.ItemBonus:
                    return true;
                case DDormanType.ItemQuest:
                    return true;
                case DDormanType.Key:
                    return true;
                case DDormanType.KeyFinal:
                    return true;
                case DDormanType.KeyMultiPiece:
                    return true;
                case DDormanType.Lock:
                    return true;
                case DDormanType.LockFinal:
                    return true;
                case DDormanType.LockMulti:
                    return true;
                case DDormanType.Nothing:
                    return true;
                case DDormanType.Start:
                    return false;
                case DDormanType.Test:
                    return true;
                case DDormanType.TestItem:
                    return true;
                case DDormanType.TestSecret:
                    return true;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override string ToString()
        {
            return $"{type}";
        }

        public override bool Equals(object other)
        {
            switch (other)
            {
                case null:
                    return false;
                case DDorman dorman:
                    return Equals(dorman);
                default:
                    return false;
            }
        }

        protected bool Equals(DDorman other)
        {
            return type == other.type;
        }

        public override int GetHashCode()
        {
            return (int) type;
        }
    }
}