﻿#region

using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using SpaceAPI.Data.Contexts;
using SpaceAPI.Data.Models;
using SpaceAPI.Data.Models.API;
using SpaceAPI.Models.API;

#endregion

namespace SpaceAPI.Controllers
{
    public class StatusController : ApiController
    {
        private LogContext _context;

        public StatusController(LogContext context)
        {
            _context = context;
        }

        public StatusController()
        {
            _context = new LogContext();
        }

        [Route("api/status")]
        [HttpGet]
        public IHttpActionResult Get()
        {
            using (_context = new LogContext())
            {
                Root root = GetRootObject();
                var lastLog = _context.StateLogs.OrderByDescending(x => x.Id).FirstOrDefault();
                bool open = lastLog != null && lastLog.Open;
                root.State.Open = open;
                return Ok(root);
            }
        }

        private Root GetRootObject()
        {
            var reportChannels = new List<string> { "email" };
            Root root = new Root
            {
                Api = "0.13",
                Cache = new Cache
                {
                    Schedule = "h.01"
                },
                Contact = new Contact
                {
                    Email = "info@brixel.be",
                    Irc = "irc://irc.freenode.net/brixel",
                    Ml = "brixel-public@discuss.brixel.be",
                    Twitter = "@hs_hasselt"
                },
                IssueReportChannels = reportChannels.ToArray(),
                Location = new Location
                {
                    Address = "Spalbeekstraat 34, 3510 Spalbeek, Belgium",
                    Lat = (float)50.9509978,
                    Lon = (float)5.2305834
                },
                Logo = "http://wiki.brixel.be/images/Brixel_Logo.png",
                Projects = new[] { "http://wiki.brixel.be/w/Category:Projects" },
                Space = "Brixel",
                State  = new State()
                {
                    Open = false,
                },
                Spacefed = new Spacefed
                {
                    Spacenet = false,
                    Spacephone = false,
                    Spacesaml = false
                },
                Url = "http://brixel.be"
            };
            return root;
        }


        [HttpGet]
        public IHttpActionResult Open()
        {
            var root = GetRootObject();
            root.State = new State()
            {
                Open = true
            };

            using (var context = new LogContext())
            {
                var stateLogging = new StateLog() {Open = true};
                context.StateLogs.Add(stateLogging);
                context.SaveChanges();
            } 

            return Ok(root);
        }

        [HttpGet]
        public IHttpActionResult Close()
        {
            var root = GetRootObject();
            root.State = new State()
            {
                Open = false
            };
            using (var context = new LogContext())
            {
                var stateLogging = new StateLog() { Open = false };
                context.StateLogs.Add(stateLogging);
                context.SaveChanges();
            } 
            return Ok(root);
        }
    }
}