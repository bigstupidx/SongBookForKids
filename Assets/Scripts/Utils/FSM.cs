using System;

namespace Mandarin {
    public class FSM {

        private Action<FSM>[] states;
        private int state;

        public FSM(int numStates) {
            states = new Action<FSM>[numStates];
            state = 0;
        }

        public FSM AddState(int i, Action<FSM> stateCb) {
            states[i] = stateCb;
            return this;
        }

        public void SetState(int i) {
            state = i;
        }

        public void Run() {
            states[state](this);
        }
    }
}
