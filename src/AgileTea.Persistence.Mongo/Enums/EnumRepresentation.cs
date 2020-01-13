namespace AgileTea.Persistence.Mongo.Enums
{
    /// <summary>
    /// Represent the constrained options for how enums should represented by the Mongo Db.
    /// </summary>
    public enum EnumRepresentation
    {
        /// <summary>
        /// Represents enums in a mongo database as the integer equivalent
        /// </summary>
        Numeric,

        /// <summary>
        /// Represents enums as their names
        /// </summary>
        String
    }
}
