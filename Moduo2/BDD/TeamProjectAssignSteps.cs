using System;
using TechTalk.SpecFlow;

namespace BDD
{
    [Binding]
    public class TeamProjectAssignSteps
    {
        [Given(@"I have the service methods for assigning")]
        public void GivenIHaveTheServiceMethodsForAssigning()
        {
            ScenarioContext.Current.Pending();
        }
        
        [When(@"I choose a team and press button")]
        public void WhenIChooseATeamAndPressButton()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"the project is sent to the team leader")]
        public void ThenTheProjectIsSentToTheTeamLeader()
        {
            ScenarioContext.Current.Pending();
        }
    }
}
