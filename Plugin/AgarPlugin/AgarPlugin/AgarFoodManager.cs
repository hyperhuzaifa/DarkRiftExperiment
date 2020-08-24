using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DarkRift;
using DarkRift.Server;

namespace AgarPlugin
{
    class AgarFoodManager : Plugin
    {

        const float MAP_WIDTH = 20;

        public override bool ThreadSafe => false;

        public override Version Version => new Version(1, 0, 0);

        public IEnumerable<FoodItem> Food => foodItems;


        public HashSet<FoodItem> foodItems = new HashSet<FoodItem>();
        public AgarFoodManager(PluginLoadData pluginLoadData) : base(pluginLoadData)
        {
            Random r = new Random();

            for (int i = 0; i < 20; i++)
            {
                foodItems.Add(new FoodItem((ushort)(i), (float)r.NextDouble() * MAP_WIDTH - MAP_WIDTH / 2,
                    (float)r.NextDouble() * MAP_WIDTH - MAP_WIDTH / 2,
                    (byte)r.Next(0, 200),
                    (byte)r.Next(0, 200),
                    (byte)r.Next(0, 200)));
            }

            ClientManager.ClientConnected += ClientConnected;
        }

        void ClientConnected(object sender, ClientConnectedEventArgs e)
        {

            using (DarkRiftWriter newFoodWriter = DarkRiftWriter.Create())
            {
                foreach(FoodItem f in foodItems)
                {
                    newFoodWriter.Write(f.ID);
                    newFoodWriter.Write(f.X);
                    newFoodWriter.Write(f.Y);
                    newFoodWriter.Write(f.ColorR);
                    newFoodWriter.Write(f.ColorG);
                    newFoodWriter.Write(f.ColorB);
                }

                using (Message newPlayerMessage = Message.Create(Tags.FoodSpawnTag, newFoodWriter))
                        e.Client.SendMessage(newPlayerMessage, SendMode.Reliable);
            }

        }

        public void Eat(FoodItem foodItem)
        {
            Random r = new Random();

            foodItem.X = (float)r.NextDouble() * MAP_WIDTH - MAP_WIDTH / 2;
            foodItem.Y = (float)r.NextDouble() * MAP_WIDTH - MAP_WIDTH / 2;

            using (DarkRiftWriter foodWriter = DarkRiftWriter.Create())
            {
                foodWriter.Write(foodItem.ID);
                foodWriter.Write(foodItem.X);
                foodWriter.Write(foodItem.Y);

                using (Message playerMessage = Message.Create(Tags.MoveFoodTag, foodWriter))
                {
                    foreach (IClient client in ClientManager.GetAllClients())
                        client.SendMessage(playerMessage, SendMode.Reliable);
                }
            }
        }
    }
}
