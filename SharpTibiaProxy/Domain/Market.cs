using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpTibiaProxy.Domain
{
    public class Market
    {
        private Client client;

        public Market(Client client)
        {
            this.client = client;
        }

        public ulong AccountBalance { get; set; }
        public byte ActiveOffers { get; set; }
        public List<DepotObject> DepotContent = new List<DepotObject>();

        public ushort BrowseType { get; set; }
        public List<string> BrowseDetails = new List<string>();
        public List<OfferStatistic> OfferStatistics = new List<OfferStatistic>();
        public List<Offer> Offers = new List<Offer>();

        public void Leave()
        {
            client.ProtocolWorld.SendServerMarketLeave();
        }

        public void Browse(ushort itemid)
        {
            client.ProtocolWorld.SendServerMarketBrowse(itemid);
        }
    }

    public class DepotObject
    {
        public ushort Id { get; set; }
        public ushort Count { get; set; }

        public DepotObject(ushort id, ushort count)
        {
            this.Id = id;
            this.Count = count;
        }
    }

    public class OfferStatistic
    {
        public DateTime Timestamp { get; set; }
        public OfferKind Kind { get; set; }
        public uint TotalTransactions { get; set; }
        public uint MinimumPrice { get; set; }
        public uint TotalPrice { get; set; }
        public uint MaximumPrice { get; set; }

        public OfferStatistic(double timestamp, OfferKind kind, uint totaltransactions, uint totalprice, uint maximumprice, uint minimumprice)
        {
            double ts = Math.Floor(timestamp / 86400) * 86400;
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0).ToUniversalTime();
            dt = dt.AddMilliseconds(ts);
            this.Timestamp = dt;
            this.Kind = kind;
            this.TotalTransactions = totaltransactions;
            this.TotalPrice = totalprice;
            this.MaximumPrice = maximumprice;
            this.MinimumPrice = minimumprice;
        }
    }

    public class Offer
    {
        public OfferId OfferId { get; set; }
        public OfferKind Kind { get; set; }
        public uint TypeId { get; set; }
        public uint Amount { get; set; }
        public uint PiecePrice { get; set; }
        public string Character { get; set; }
        public TerminationType TerminationType { get; set; }

        public Offer(OfferId offerid, OfferKind kind, uint typeid, uint amount, uint pieceprice, string character, TerminationType terminationtype)
        {
            this.OfferId = offerid;
            this.Kind = kind;
            this.TypeId = typeid;
            this.Amount = amount;
            this.PiecePrice = pieceprice;
            this.Character = character;
            this.TerminationType = terminationtype;
        }
    }

    public class OfferId
    {
        public uint Counter { get; set; }
        public DateTime Timestamp { get; set; }

        public OfferId(uint timestamp, uint counter)
        {
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0).ToUniversalTime();
            dt = dt.AddHours(3);
            dt = dt.AddSeconds(timestamp);
            this.Timestamp = dt;
            this.Counter = counter;
        }
    }

    public enum OfferKind : uint
    {
        Buy = 0,
        Sell = 1
    }

    public enum TerminationType : uint
    {
        Active = 0,
        Canceled = 1,
        Expired = 2,
        Accepted = 3
    }
}
