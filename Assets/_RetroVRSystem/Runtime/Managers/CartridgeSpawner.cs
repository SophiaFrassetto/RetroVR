using System.IO;
using UnityEngine;


namespace retrovr.system
{
    public class CartridgeSpawner : MonoBehaviour
    {
        public GameObject cartridgePrefab;
        public Transform spawnRoot;

        void Start()
        {
            SpawnAll();
        }

        void SpawnAll()
        {
            string root = Path.Combine(Application.persistentDataPath, "roms");

            foreach (var dir in Directory.GetDirectories(root))
            {
                string sub = Path.GetFileName(dir);
                foreach (var file in Directory.GetFiles(dir))
                {
                    var def = ScriptableObject.CreateInstance<CartridgeDefinition>();
                    def.romName = Path.GetFileNameWithoutExtension(file);
                    def.romSubfolder = sub;
                    def.extension = Path.GetExtension(file);

                    var obj = Instantiate(cartridgePrefab, spawnRoot.position, Quaternion.identity);
                    obj.GetComponent<CartridgeInstance>().cartridgeDefinition = def;
                    obj.GetComponent<CartridgeInstance>().SetDisplayName(def.romName);
                }
            }
        }
    }
}