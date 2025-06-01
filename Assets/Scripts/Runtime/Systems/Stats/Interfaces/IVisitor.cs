using UnityEngine;

namespace Universal.Runtime.Systems.Stats
{
    public interface IVisitor
    {
        void Visit<T>(T visitable) where T : Component, IVisitable;
    }
}