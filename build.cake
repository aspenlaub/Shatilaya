#load "solution.cake"

var target = Argument("target", "Default");
Task("Default")
  .Does(() =>
{
});
RunTarget(target);