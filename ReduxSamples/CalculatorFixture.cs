using Storyteller.Redux;
using StoryTeller;

namespace ReduxSamples
{
    public class CalculatorFixture : ReduxFixture
    {
        public void GetInitialState()
        {
            this.ForceRefetchOfState().Wait();
        }

        [SendJson("INCREMENT_COUNT")]
        public void Increment()
        {
            
        }

        // SAMPLE: CheckJsonValue
        public IGrammar CheckValue()
        {
            return CheckJsonValue<int>("$.counter.count", "The current counter should be {number}");
        }
        // ENDSAMPLE
    }
}