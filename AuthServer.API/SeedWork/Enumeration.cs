using System;

namespace Expertec.Sigeco.AuthServer.API.SeedWork
{
    public abstract class Enumeration : IComparable
    {
        public string Nombre { get; private set; }

        public int Id { get; private set; }

        protected Enumeration(){ }

        protected Enumeration(int id, string nombre)
        {
            Id = id;
            Nombre = nombre;
        }

        public override string ToString() => Nombre;

        public int CompareTo(object other) => Id.CompareTo(((Enumeration)other).Id);
    }
}