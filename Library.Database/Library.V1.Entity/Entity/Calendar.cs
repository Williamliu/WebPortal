using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using Library.V1.Common;

namespace Library.V1.Entity
{
    public class Calendar
    {
        public Calendar()
        {
            this.Data = new Dictionary<string, object>();
            this.Events = new List<Event>();
            this.Error = new Error();
        }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public bool IsActive { get; set; }
        public Dictionary<string, object> Data { get; set; }
        public IList<Event> Events { get; set; }
        public Error Error { get; set; }

        public void Add(Event evt)
        {
            this.Events.Add(evt);
        }
        public void Add(int Id, DateTime dt, TimeSpan from, TimeSpan to, string title, string detail = "", bool status = false, int state = 0)
        {
            Event evt = new Event
            {
                Id = Id,
                Date = dt,
                Title = title,
                Detail = detail,
                Status = status,
                State = state,
                From = from,
                To = to,
                YY = dt.Year.ToString(),
                MM = (dt.Month - 1).ToString(),
                DD = dt.Day.ToString()
            };
            this.Add(evt);
        }
    }

    public class Event
    {
        public Event() { }
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string YY { get; set; }
        public string MM { get; set; }
        public string DD { get; set; }
        public TimeSpan From { get; set; }
        public TimeSpan To { get; set; }
        public bool Status { get; set; }
        public int State { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
    }
}
