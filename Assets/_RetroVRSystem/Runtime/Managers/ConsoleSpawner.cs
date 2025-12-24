using UnityEngine;

namespace retrovr.system
{
    public class ConsoleSpawner : MonoBehaviour
    {
        public GameObject consolePrefab;
        public Transform spawnRoot;
        public ConsoleDefinition[] availableConsoles;

        void Start()
        {
            foreach (var def in availableConsoles)
            {
                var obj = Instantiate(consolePrefab, spawnRoot.position, Quaternion.identity);
                obj.GetComponent<ConsoleInstance>().SetDefinition(def);
            }
        }
    }
}