var target = Argument("target", "Default");

Task("Default")
.Does(() => {
	Information("That is what I call a cake!");
});

RunTarget(target);