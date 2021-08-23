﻿using RogersSierra.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogersSierra.Handlers
{
    class BrakeHandler : Handler
    {
        /// <summary>
        /// How fast train brakes.
        /// </summary>
        public float BrakeMultiplier = 2f;

        public float Force { get; private set; }

        public BrakeHandler(Train train) : base(train)
        {

        }

        public override void OnInit()
        {

        }

        public override void OnTick()
        {

        }

        public override void Dispose()
        {

        }
    }
}