namespace CommonMarkSharp
{
    public abstract class Part
    {
        public virtual void Accept(CommonMarkVisitor visitor)
        {
            visitor.VisitPart(this);
        }
    }
}
