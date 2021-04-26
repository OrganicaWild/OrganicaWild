using System;

namespace Assets.Scripts.Demo.Pipeline.PipelineSteps
{
    class GameWorldSupplier : Attribute {}
    class AreaSupplier : Attribute {}

    class AreaTypeAssignmentSupplier : Attribute {}

    class AreaConnectionSupplier : Attribute { }

    class LandmarkSupplier : Attribute {}

    class MainPathSupplier : Attribute {}

    class SubAreaSupplier : Attribute {}

    class ClutteringSupplier : Attribute {}
}
