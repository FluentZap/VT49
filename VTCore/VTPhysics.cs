using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.CollisionDetection;
using BepuPhysics.Constraints;
using BepuUtilities;
using BepuUtilities.Memory;
using Quat = BepuUtilities.Quaternion;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using static SDL2.SDL;

namespace VT49
{

  unsafe struct NarrowPhaseCallbacks : INarrowPhaseCallbacks
  {
    /// <summary>
    /// Performs any required initialization logic after the Simulation instance has been constructed.
    /// </summary>
    /// <param name="simulation">Simulation that owns these callbacks.</param>
    public void Initialize(Simulation simulation)
    {
      //Often, the callbacks type is created before the simulation instance is fully constructed, so the simulation will call this function when it's ready.
      //Any logic which depends on the simulation existing can be put here.
    }

    /// <summary>
    /// Chooses whether to allow contact generation to proceed for two overlapping collidables.
    /// </summary>
    /// <param name="workerIndex">Index of the worker that identified the overlap.</param>
    /// <param name="a">Reference to the first collidable in the pair.</param>
    /// <param name="b">Reference to the second collidable in the pair.</param>
    /// <returns>True if collision detection should proceed, false otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool AllowContactGeneration(int workerIndex, CollidableReference a, CollidableReference b)
    {
      //Before creating a narrow phase pair, the broad phase asks this callback whether to bother with a given pair of objects.
      //This can be used to implement arbitrary forms of collision filtering. See the RagdollDemo or NewtDemo for examples.
      return true;
    }

    /// <summary>
    /// Chooses whether to allow contact generation to proceed for the children of two overlapping collidables in a compound-including pair.
    /// </summary>
    /// <param name="pair">Parent pair of the two child collidables.</param>
    /// <param name="childIndexA">Index of the child of collidable A in the pair. If collidable A is not compound, then this is always 0.</param>
    /// <param name="childIndexB">Index of the child of collidable B in the pair. If collidable B is not compound, then this is always 0.</param>
    /// <returns>True if collision detection should proceed, false otherwise.</returns>
    /// <remarks>This is called for each sub-overlap in a collidable pair involving compound collidables. If neither collidable in a pair is compound, this will not be called.
    /// For compound-including pairs, if the earlier call to AllowContactGeneration returns false for owning pair, this will not be called. Note that it is possible
    /// for this function to be called twice for the same subpair if the pair has continuous collision detection enabled; 
    /// the CCD sweep test that runs before the contact generation test also asks before performing child pair tests.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool AllowContactGeneration(int workerIndex, CollidablePair pair, int childIndexA, int childIndexB)
    {
      //This is similar to the top level broad phase callback above. It's called by the narrow phase before generating
      //subpairs between children in parent shapes. 
      //This only gets called in pairs that involve at least one shape type that can contain multiple children, like a Compound.
      return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void ConfigureMaterial(out PairMaterialProperties pairMaterial)
    {
      //The engine does not define any per-body material properties. Instead, all material lookup and blending operations are handled by the callbacks.
      //For the purposes of this demo, we'll use the same settings for all pairs.
      //(Note that there's no bounciness property! See here for more details: https://github.com/bepu/bepuphysics2/issues/3)
      pairMaterial.FrictionCoefficient = 1f;
      pairMaterial.MaximumRecoveryVelocity = 2f;
      pairMaterial.SpringSettings = new SpringSettings(30, 1);
    }

    //Note that there is a unique callback for convex versus nonconvex types. There is no fundamental difference here- it's just a matter of convenience
    //to avoid working through an interface or casting.
    //For the purposes of the demo, contact constraints are always generated.
    /// <summary>
    /// Provides a notification that a manifold has been created for a pair. Offers an opportunity to change the manifold's details. 
    /// </summary>
    /// <param name="workerIndex">Index of the worker thread that created this manifold.</param>
    /// <param name="pair">Pair of collidables that the manifold was detected between.</param>
    /// <param name="manifold">Set of contacts detected between the collidables.</param>
    /// <param name="pairMaterial">Material properties of the manifold.</param>
    /// <returns>True if a constraint should be created for the manifold, false otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe bool ConfigureContactManifold(int workerIndex, CollidablePair pair, NonconvexContactManifold* manifold, out PairMaterialProperties pairMaterial)
    {
      ConfigureMaterial(out pairMaterial);
      return true;
    }

    /// <summary>
    /// Provides a notification that a manifold has been created for a pair. Offers an opportunity to change the manifold's details. 
    /// </summary>
    /// <param name="workerIndex">Index of the worker thread that created this manifold.</param>
    /// <param name="pair">Pair of collidables that the manifold was detected between.</param>
    /// <param name="manifold">Set of contacts detected between the collidables.</param>
    /// <param name="pairMaterial">Material properties of the manifold.</param>
    /// <returns>True if a constraint should be created for the manifold, false otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe bool ConfigureContactManifold(int workerIndex, CollidablePair pair, ConvexContactManifold* manifold, out PairMaterialProperties pairMaterial)
    {
      ConfigureMaterial(out pairMaterial);
      return true;
    }

    /// <summary>
    /// Provides a notification that a manifold has been created between the children of two collidables in a compound-including pair.
    /// Offers an opportunity to change the manifold's details. 
    /// </summary>
    /// <param name="workerIndex">Index of the worker thread that created this manifold.</param>
    /// <param name="pair">Pair of collidables that the manifold was detected between.</param>
    /// <param name="childIndexA">Index of the child of collidable A in the pair. If collidable A is not compound, then this is always 0.</param>
    /// <param name="childIndexB">Index of the child of collidable B in the pair. If collidable B is not compound, then this is always 0.</param>
    /// <param name="manifold">Set of contacts detected between the collidables.</param>
    /// <returns>True if this manifold should be considered for constraint generation, false otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ConfigureContactManifold(int workerIndex, CollidablePair pair, int childIndexA, int childIndexB, ConvexContactManifold* manifold)
    {
      return true;
    }

    /// <summary>
    /// Releases any resources held by the callbacks. Called by the owning narrow phase when it is being disposed.
    /// </summary>
    public void Dispose()
    {
    }
  }

  public struct PoseIntegratorCallbacks : IPoseIntegratorCallbacks
  {
    public Vector3 Gravity;
    Vector3 gravityDt;

    public Vector2 Friction;
    Vector2 frictionDt;

    /// <summary>
    /// Gets how the pose integrator should handle angular velocity integration.
    /// </summary>
    public AngularIntegrationMode AngularIntegrationMode => AngularIntegrationMode.Nonconserving; //Don't care about fidelity in this demo!

    public PoseIntegratorCallbacks(Vector3 gravity, Vector2 friction) : this()
    {
      Gravity = gravity;
      Friction = friction;
    }

    /// <summary>
    /// Called prior to integrating the simulation's active bodies. When used with a substepping timestepper, this could be called multiple times per frame with different time step values.
    /// </summary>
    /// <param name="dt">Current time step duration.</param>
    public void PrepareForIntegration(float dt)
    {
      //No reason to recalculate gravity * dt for every body; just cache it ahead of time.
      gravityDt = Gravity * dt;
      frictionDt = Friction * dt;
    }

    /// <summary>
    /// Callback called for each active body within the simulation during body integration.
    /// </summary>
    /// <param name="bodyIndex">Index of the body being visited.</param>
    /// <param name="pose">Body's current pose.</param>
    /// <param name="localInertia">Body's current local inertia.</param>
    /// <param name="workerIndex">Index of the worker thread processing this body.</param>
    /// <param name="velocity">Reference to the body's current velocity to integrate.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void IntegrateVelocity(int bodyIndex, in RigidPose pose, in BodyInertia localInertia, int workerIndex, ref BodyVelocity velocity)
    {
      //Note that we avoid accelerating kinematics. Kinematics are any body with an inverse mass of zero (so a mass of ~infinity). No force can move them.
      if (localInertia.InverseMass > 0)
      {
        velocity.Linear = velocity.Linear + gravityDt;

        velocity.Angular = velocity.Angular - (velocity.Angular * frictionDt.X);
        velocity.Linear = velocity.Linear - (velocity.Linear * frictionDt.Y);
      }
    }

  }



  public class VTPhysics : IDisposable
  {
    SWSimulation _sws;
    BufferPool bufferPool = new BufferPool();
    Simulation sim;
    Box box = new Box(1, 1, 1);
    int body = 0;
    int station = 0;
    int asteroid = 0;

    public VTPhysics(ref SWSimulation sws)
    {
      _sws = sws;
      sim = Simulation.Create(bufferPool, new NarrowPhaseCallbacks(), new PoseIntegratorCallbacks(new Vector3(0, 0, 0), new Vector2(5f, 1f)));

      // ConvexHull VT49 = new ConvexHull(MeshLoader.LoadPointsFromFile(bufferPool, "VT49CH.obj"), bufferPool, out _sws.PCShip.LocationOffset);
      Mesh VT49 = MeshLoader.LoadTriangleMesh(bufferPool, "VT49TRI.obj", Vector3.One);
      box.ComputeInertia(1, out var sphereInertia);

      // body = sim.Bodies.Add(BodyDescription.CreateDynamic(new Vector3(0, 5, 0), sphereInertia, new CollidableDescription(sim.Shapes.Add(box), 0.1f), new BodyActivityDescription(0.01f)));      
      // body = sim.Bodies.Add(BodyDescription.CreateDynamic(new RigidPose(new Vector3(0, 0, 0), new BepuUtilities.Quaternion(0, 1, 0, 0)), sphereInertia, new CollidableDescription(sim.Shapes.Add(box), 0.1f), new BodyActivityDescription(0.01f)));
      body = sim.Bodies.Add(BodyDescription.CreateDynamic(new RigidPose(new Vector3(0, 0, 0) + _sws.PCShip.Location, new BepuUtilities.Quaternion(0, 1, 0, 0)), sphereInertia, new CollidableDescription(sim.Shapes.Add(VT49), 0.1f), new BodyActivityDescription(0.01f)));


      // sim.Statics.Add(new StaticDescription(new Vector3(0, 0, 30), new CollidableDescription(sim.Shapes.Add(new Box(1, 1, 1)), 0.1f)));
      Mesh XQ6 = MeshLoader.LoadTriangleMesh(bufferPool, "XQ6TRI.obj", Vector3.One);

      station = sim.Statics.Add(new StaticDescription(new Vector3(0, 0, 100) + _sws.Station.Location, Quat.CreateFromYawPitchRoll(0, 0, 0), new CollidableDescription(sim.Shapes.Add(XQ6), 0.1f)));

      Mesh Asteroid_Type1 = MeshLoader.LoadTriangleMesh(bufferPool, "Asteroid_Type1.obj", new Vector3(10, 10, 10));

      asteroid = sim.Statics.Add(new StaticDescription(new Vector3(15, 0, 15), Quat.CreateFromYawPitchRoll(0, 0, 0), new CollidableDescription(sim.Shapes.Add(Asteroid_Type1), 0.1f)));

      // _sws.StationVectors = MeshLoader.LoadPointsFromFile("XQ6CH.obj");
      // for (int i = 0; i < 100; ++i)
      // {
      //   //Multithreading is pretty pointless for a simulation of one ball, but passing a IThreadDispatcher instance is all you have to do to enable multithreading.
      //   //If you don't want to use multithreading, don't pass a IThreadDispatcher.
      //   sim.Timestep(0.01f);
      // }

    }


    public void Update()
    {
      sim.Awakener.AwakenBody(body);

      BodyReference bref = new BodyReference(body, sim.Bodies);

      // Vector3 vel = new Vector3(
      //   _sws.ConsoleInput.IsDown(ListOf_ConsoleInputs.FlightStickRIGHT) == true ? -0.1f :
      //   _sws.ConsoleInput.IsDown(ListOf_ConsoleInputs.FlightStickLEFT) == true ? 0.1f :
      //   0,
      //   _sws.ConsoleInput.IsDown(ListOf_ConsoleInputs.FlightStickUP) == true ? -0.1f :
      //   _sws.ConsoleInput.IsDown(ListOf_ConsoleInputs.FlightStickDOWN) == true ? 0.1f :
      //    0,
      //     (_sws.LeftInput.FlightStick.Throttle + 32767) * 0.000001f
      //    );

      Vector3 vel = new Vector3(
              _sws.LeftInput.FlightStick.HAT == SDL_HAT_RIGHT ? -0.1f :
              _sws.LeftInput.FlightStick.HAT == SDL_HAT_LEFT ? 0.1f :
              0,
              _sws.LeftInput.FlightStick.HAT == SDL_HAT_UP ? -0.1f :
              _sws.LeftInput.FlightStick.HAT == SDL_HAT_DOWN ? 0.1f :
               0,
                (_sws.LeftInput.FlightStick.Throttle + 32767) * 0.0001f
               );

      Vector3 rotation = _sws.LeftInput.FlightStick.Axis * 0.0000004f;

      rotation = Quat.Transform(rotation, bref.Pose.Orientation);
      bref.ApplyAngularImpulse(rotation);

      vel = Quat.Transform(-vel, bref.Pose.Orientation);
      bref.ApplyLinearImpulse(vel);

      if (_sws.ConsoleInput.IsDown(ListOf_ConsoleInputs.LEDButton1) || _sws.LeftInput.FlightStick.Buttons.Triggered(0))
      {
        bref.Pose.Position = Vector3.Zero;
        bref.Pose.Orientation = new BepuUtilities.Quaternion(0, 1, 0, 0);
        bref.Velocity.Linear = Vector3.Zero;
        bref.Velocity.Angular = Vector3.Zero;
      }
      // sim.Shapes.GetShape(bref.Collidable.Shape)
      // System.Console.WriteLine(bref.Velocity.Angular.ToString());

      _sws.PCShip.Location = bref.Pose.Position;
      _sws.PCShip.Rotation = bref.Pose.Orientation;
      sim.Timestep(0.01f);
    }


    public void Dispose()
    {
      sim.Dispose();
      bufferPool.Clear();
    }

  }
}