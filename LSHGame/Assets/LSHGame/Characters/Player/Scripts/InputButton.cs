using UnityEngine;


namespace LSHGame.PlayerN
{
    [System.Serializable]
    public class InputButton
    {
        [SerializeField]
        private InputCondition[] inputConditions = new InputCondition[1];

        [SerializeField]
        private bool needRepress = false;

        [SerializeField]
        private float resetTimer;

        private float lastActiveTimer = float.NegativeInfinity;

        private bool wasRealeasedSinceActivate = true;

        private bool wasButtonPressedAndActive = false;

        //public InputButton(string keyCode,float beforeTime,float afterTime)
        //{
        //    this.keyCode = keyCode;
        //    this.beforeConDeltaTime = beforeTime;
        //    this.afterConDeltaTime = afterTime;
        //}

        public bool Check(bool buttonDown, bool condition, int conIndex = 0)
        {
            bool tmp = false;
            return Check(buttonDown, condition, ref tmp, conIndex);
        }

        public bool Check(bool buttonDown, bool condition, ref bool wasReleased, int conIndex = 0)
        {
            if (CheckWithReset(buttonDown, condition, conIndex))
            {
                if (!buttonDown)
                    wasReleased = true;
                else
                    wasButtonPressedAndActive = true;

                //Debug.Log($"Check Succes\nButtonDown: {buttonDown}\tCondition: {condition}" +
                //    $"\nWasReleased: {wasReleased}\tConIndex: {conIndex}\n{inputConditions[conIndex].ToString()}\n" +
                //    $"lastActiveTimer: {lastActiveTimer}\twasReleasedSinceActive: {wasRealeasedSinceActivate}\n" +
                //    $"wasButtonPressedSinceActive: {wasButtonPressedAndActive}");
                return true;
            }

            if (!buttonDown && wasButtonPressedAndActive)
            {
                wasReleased = true;
                wasButtonPressedAndActive = false;
            }

            //Debug.Log($"Check Failed\nButtonDown: {buttonDown}\tCondition: {condition}" +
            //        $"\nWasReleased: {wasReleased}\tConIndex: {conIndex}\n{inputConditions[conIndex].ToString()}\n" +
            //        $"lastActiveTimer: {lastActiveTimer}\twasReleasedSinceActive: {wasRealeasedSinceActivate}\n" +
            //        $"wasButtonPressedSinceActive: {wasButtonPressedAndActive}");

            return false;
        }

        public void Reset()
        {
            ResetAllInputConditions();
            lastActiveTimer = float.NegativeInfinity;

            wasRealeasedSinceActivate = true;

            wasButtonPressedAndActive = false;
        }

        private void ResetAllInputConditions()
        {
            foreach (var inputCondition in inputConditions)
                inputCondition.Reset();
        }


        private bool CheckWithReset(bool buttonDown, bool condition, int conIndex)
        {
            condition &= Time.fixedTime - lastActiveTimer >= resetTimer;

            wasRealeasedSinceActivate |= !buttonDown;
            if (needRepress && !wasRealeasedSinceActivate)
                buttonDown = false;

            if (CheckWithThreshold(buttonDown, condition, conIndex))
            {
                lastActiveTimer = Time.time;
                wasRealeasedSinceActivate = false;
                return true;
            }
            return false;
        }

        private bool CheckWithThreshold(bool buttonDown, bool condition, int conIndex)
        {
            if (inputConditions[conIndex].IsInRange(buttonDown, condition))
            {
                ResetAllInputConditions();
                return true;
            }

            if (condition)
                inputConditions[conIndex].ClockAfter();

            if (buttonDown)
                inputConditions[conIndex].ClockBefore();

            return false;
        }

        [System.Serializable]
        public class InputCondition
        {
            [SerializeField]
            private float beforeConDeltaTime;
            [SerializeField]
            private float afterConDeltaTime;

            private float beforeConTimer = float.NegativeInfinity;
            private float afterConTimer = float.NegativeInfinity;

            public void Reset()
            {
                beforeConTimer = float.NegativeInfinity;
                afterConTimer = float.NegativeInfinity;
            }

            public void ClockBefore()
            {
                beforeConTimer = Time.fixedTime + beforeConDeltaTime;
                afterConTimer = float.NegativeInfinity;
            }

            public void ClockAfter()
            {
                afterConTimer = Time.fixedTime + afterConDeltaTime;
                beforeConTimer = float.NegativeInfinity;
            }

            public bool IsInRange(bool buttonDown, bool condition)
            {
                if (condition && buttonDown)
                    return true;

                if (buttonDown && afterConTimer > Time.fixedTime)
                    return true;

                if (condition && beforeConTimer >= Time.fixedTime)
                    return true;

                return false;
            }

            public override string ToString()
            {
                return $"InputCondition:\nBeforConTimer: {beforeConTimer}\tAfterConTimer: {afterConTimer}";
            }
        }
    }
}
