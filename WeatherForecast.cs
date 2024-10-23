using System.Diagnostics;

namespace DnC
{
    public struct Vector3
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public Vector3(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override string ToString()
        {
            return $"{X}, {Y}, {Z}";
        }
    }

    public class Entity
    {
        public Guid Id { get; }

        public Entity()
        {
            Id = Guid.NewGuid();
        }
    }

    public interface Component { }

    public class PositionComponent : Component
    {
        public Vector3 Position { get; set; }

        public PositionComponent(float x, float y, float z)
        {
            Position = new Vector3(x, y, z);
        }
    }

    public class ComponentManager
    {
        private Dictionary<Guid, Dictionary<Type, Component>> _entityComponents;

        public ComponentManager()
        {
            _entityComponents = new Dictionary<Guid, Dictionary<Type, Component>>();
        }

        public void AddComponent<T>(Guid entityId, T component) where T : Component
        {
            if (!_entityComponents.ContainsKey(entityId))
            {
                _entityComponents[entityId] = new Dictionary<Type, Component>();
            }
            _entityComponents[entityId][typeof(T)] = component;
        }

        public T GetComponent<T>(Guid entityId) where T : Component
        {
            if (_entityComponents.ContainsKey(entityId) && _entityComponents[entityId].ContainsKey(typeof(T)))
            {
                return (T)_entityComponents[entityId][typeof(T)];
            }

            return default;
        }

        public bool HasComponent<T>(Guid entityId) where T : Component
        {
            return _entityComponents.ContainsKey(entityId) && _entityComponents[entityId].ContainsKey(typeof(T));
        }
    }

    public abstract class System
    {
        public abstract void Update(ComponentManager componentManager, IEnumerable<Guid> entities, float delta_time);
    }

    public class MovementSystem : System
    {
        public bool Logging = false;

        public override void Update(ComponentManager componentManager, IEnumerable<Guid> entities, float delta_time)
        {
            foreach (var entityId in entities)
            {
                if (componentManager.HasComponent<PositionComponent>(entityId))
                {
                    UpdateEntity(componentManager, entityId, delta_time);
                }
            }
        }

        private void UpdateEntity(ComponentManager componentManager, Guid entityId, float delta_time)
        {
            var positionComponent = componentManager.GetComponent<PositionComponent>(entityId);

            // Move the position
            positionComponent.Position = new Vector3(
                positionComponent.Position.X + 1.0f * delta_time,
                positionComponent.Position.Y,
                positionComponent.Position.Z
            );

            if (Logging)
            {
                // Log the position
                Debug.WriteLine(positionComponent.Position.ToString());
            }
        }
    }

    public class Game
    {
        protected Dictionary<Guid, Entity> Entities = new();
        protected MovementSystem _movementSystem = new();
        protected ComponentManager _componentManager;
        protected Stopwatch _stopwatch = new();
        protected double _lastFrameTime;

        public Game(ComponentManager component_manager)
        {
            _movementSystem.Logging = true;
            _componentManager = component_manager;
        }

        public void Update()
        {
            double currentFrameTime = _stopwatch.Elapsed.TotalSeconds;
            double deltaTime = currentFrameTime - _lastFrameTime;

            _movementSystem.Update(_componentManager, GetAllEntityIds(), (float) deltaTime);
        }
        public void AddEntity(Entity entity)
        {
            Entities[entity.Id] = entity;
        }

        public void RemoveEntity(Guid entity_id)
        {
            Entities.Remove(entity_id);
        }
        public IEnumerable<Guid> GetAllEntityIds()
        {
            return Entities.Keys;
        }

        public Entity GetEntityById(Guid entity_id) {
            return Entities[entity_id];
        }

        public void StartGameLoop()
        {
            _lastFrameTime = _stopwatch.Elapsed.TotalSeconds;
            _stopwatch.Start();

            for (;;)
            {
                Console.WriteLine("WRITING");
                Update();
                Thread.Sleep(16);
            }
        }
    }
}
