using System;

namespace WEB_ProyectoFinal_Grupo3.Models
{
    public enum TiposMembresias
    {
        [EstadosAtributos(1, "Estandar")]
        Estandar,
        [EstadosAtributos(2, "Premium")]
        Premium,
        [EstadosAtributos(3, "VIP")]
        VIP
    }

    public enum EstadosPaquetes
    {
        [EstadosAtributos(1, "Disponible")]
        Disponible,
        [EstadosAtributos(2, "Vendido")]
        Vendido,
        [EstadosAtributos(3, "Reservado")]
        Reservado
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    internal class EstadosAtributosAttribute : Attribute
    {
        public int Value { get; }
        public string Text { get; }

        public EstadosAtributosAttribute(int _value, string _text)
        {
            Value = _value;
            Text = _text;
        }
    }
}
