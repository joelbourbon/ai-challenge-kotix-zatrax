using System;
using System.Collections.Generic;

namespace Ants {

	public class Location : IEquatable<Location> {

		/// <summary>
		/// Gets the row of this location.
		/// </summary>
		public int Row { get; private set; }

		/// <summary>
		/// Gets the column of this location.
		/// </summary>
		public int Col { get; private set; }

		public Location (int row, int col) {
			this.Row = row;
			this.Col = col;
		}

    public static bool operator ==(Location a, Location b)
    {
      // If both are null, or both are same instance, return true.
      if (System.Object.ReferenceEquals(a, b))
      {
        return true;
      }

      // If one is null, but not both, return false.
      if (((object)a == null) || ((object)b == null))
      {
        return false;
      }

      // Return true if the fields match:
      return a.Col == b.Col && a.Row == b.Row;
    }

    public static bool operator !=(Location a, Location b)
    {
      return !(a == b);
    }

		public override bool Equals (object obj) {
			if (ReferenceEquals (null, obj))
				return false;
			if (ReferenceEquals (this, obj))
				return true;
			if (obj.GetType() != typeof (Location))
				return false;

			return Equals ((Location) obj);
		}

		public bool Equals (Location other) {
			if (ReferenceEquals (null, other))
				return false;
			if (ReferenceEquals (this, other))
				return true;

			return other.Row == this.Row && other.Col == this.Col;
		}

		public override int GetHashCode()
		{
			unchecked {
				return (this.Row * 397) ^ this.Col;
			}
		}
	}

	public class TeamLocation : Location, IEquatable<TeamLocation> {
		/// <summary>
		/// Gets the team of this ant.
		/// </summary>
		public int Team { get; private set; }

		public TeamLocation (int row, int col, int team) : base (row, col) {
			this.Team = team;
		}

		public bool Equals(TeamLocation other) {
			return base.Equals (other) && other.Team == Team;
		}

		public override int GetHashCode()
		{
			unchecked {
				int result = this.Col;
				result = (result * 397) ^ this.Row;
				result = (result * 397) ^ this.Team;
				return result;
			}
		}
	}
	
	public class Ant : TeamLocation, IEquatable<Ant> 
  {
    public Goal AntGoal = Goal.Food;
	  public Location Destination = new Location(-1, -1);
	  public List<Location> Path = new List<Location>();

    public enum Goal
    {
      None,
      Food,
      Offense,
      Defense,
      Explore
    }

    public bool NoDestination()
    {
      if (Destination.Col == -1 && Destination.Row == -1)
        return true;
      return false;
    }

		public Ant (int row, int col, int team) : base (row, col, team) 
    {
		}

    public void setGoal(Goal iGoal)
    {
      AntGoal = iGoal;
    }

		public bool Equals (Ant other) {
			return base.Equals (other);
		}
	}

	public class AntHill : TeamLocation, IEquatable<AntHill> {
		public AntHill (int row, int col, int team) : base (row, col, team) {
		}

		public bool Equals (AntHill other) {
			return base.Equals (other);
		}
	}
}

