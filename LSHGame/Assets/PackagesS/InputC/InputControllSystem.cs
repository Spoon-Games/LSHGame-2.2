using System;
using System.Collections.Generic;
using UnityEngine;

namespace InputC
{
    internal static class InputControllSystem
    {
        private static Dictionary<Type,List<InputAgent>> agentRepo = new Dictionary<Type, List<InputAgent>>();

        internal static void RegisterListeningAgent(InputAgent agent)
        {
            if (agent.IsListening)
                return;

            List<InputAgent> listeningAgents;
            if(!agentRepo.TryGetValue(agent.GetType(),out listeningAgents)){

                listeningAgents = new List<InputAgent>();
                agentRepo.Add(agent.GetType(), listeningAgents);
            }

            agent.IsListening = true;
            listeningAgents.Add(agent);
            listeningAgents.Sort();

            SetInputKeysActive(listeningAgents);
        }

        internal static void DeregisterListeningAgent(InputAgent agent)
        {
            if (!agent.IsListening)
                return;

            List<InputAgent> listeningAgents = agentRepo[agent.GetType()];

            listeningAgents.Remove(agent);
            SetInputKeysActive(listeningAgents);

            agent.IsListening = false;
        }

        private static void SetInputKeysActive(List<InputAgent> listeningAgents)
        {
            string debug = "SetInputKeysActive Count: " + listeningAgents.Count + "\n";
            HashSet<int> blockedKeys = new HashSet<int>();

            foreach (InputAgent agent in listeningAgents)
            {
                debug += "\nAgent: " + agent.name + "\n";
                for (int i = 0; i < agent.inputKeys.Count; i++)
                {
                    var key = agent.inputKeys[i];

                    if (blockedKeys.Contains(i))
                    {
                        key.Deactivate();
                    }
                    else
                    {
                        key.Activate();
                        if (key.IsBlocking)
                            blockedKeys.Add(i);
                    }

                    debug += "\t"+key.Name + " R: " + key.IsRegistered + " B: " + key.IsBlocking + " A: " + key.IsActive + "\n";
                }
            }
            //Debug.Log(debug);
        }
    }
}
