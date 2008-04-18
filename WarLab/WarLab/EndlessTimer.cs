//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace WarLab {
//    public class GeneralTimer {
//        protected int coeff;
//        protected bool hasStarted;
//        protected bool isPaused;
//        protected int pauseStartTime;
//        protected int prevCoeff;
//        protected float speed = 0.001f;
//        protected int startTime;

//        public void Pause() {
//            if (this.hasStarted) {
//                if (!this.isPaused) {
//                    this.isPaused = true;
//                    this.prevCoeff = this.coeff;
//                    this.pauseStartTime = this.coeff * (Environment.TickCount - this.startTime);
//                    this.coeff = 0;
//                }
//                else {
//                    this.isPaused = false;
//                    this.coeff = this.prevCoeff;
//                    if (this.prevCoeff == 1) {
//                        this.startTime = Environment.TickCount - this.pauseStartTime;
//                    }
//                    else {
//                        this.startTime = Environment.TickCount + this.pauseStartTime;
//                    }
//                    this.pauseStartTime = 0;
//                }
//            }
//        }

//        public void Reverse() {
//            if (!this.isPaused) {
//                if (this.coeff == -1) {
//                    int tickCount = Environment.TickCount;
//                    this.startTime = tickCount - (this.startTime - tickCount);
//                    this.coeff = 1;
//                }
//                else {
//                    int num2 = Environment.TickCount;
//                    this.startTime = num2 + (num2 - this.startTime);
//                    this.coeff = -1;
//                }
//            }
//        }

//        public void Start() {
//            if (this.state == State.Stopped) {
//                this.startTime = Environment.TickCount;
//            }
//            if (!this.hasStarted) {
//                this.state = State.Playing;
//                if (this.isPaused) {
//                    this.Pause();
//                }
//                else {
//                    this.hasStarted = true;
//                    this.coeff = 1;
//                    this.startTime = Environment.TickCount;
//                }
//            }
//        }

//        public void Stop() {
//            this.coeff = 0;
//            this.isPaused = false;
//            this.hasStarted = false;
//            this.state = State.Stopped;
//        }

//        public float Speed {
//            get {
//                return this.speed;
//            }
//            set {
//                this.speed = value;
//            }
//        }

//        public virtual float time {
//            get {
//                return (this.speed * ((this.coeff * (Environment.TickCount - this.startTime)) + this.pauseStartTime));
//            }
//        }
//    }
//}
