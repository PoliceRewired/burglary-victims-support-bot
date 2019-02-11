using System;
using System.Collections.Generic;
using System.Text;

namespace VictimBot.Lib.Storage.DTO
{
    public class Money
    {
        public decimal Amount { get; set; } = 0.0m;
        public Currency Currency { get; set; } = Currency.GBP();
    }

    public class Currency : IComparable
    {
        public string TLA { get; set; }
        public string Symbol { get; set; }

        public string Represent(decimal amount)
        {
            return Symbol + amount;
        }

        public int CompareTo(object obj)
        {
            if (!(obj is Currency)) { return -1; }
            return TLA.CompareTo((obj as Currency).TLA);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) { return false; }
            return TLA == (obj as Currency).TLA;
        }

        public override int GetHashCode()
        {
            return TLA.GetHashCode();
        }

        public static Currency GBP()
        {
            return new Currency() { TLA = "GBP", Symbol = "£" };
        }
    }
}
