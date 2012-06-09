

function handleDataFromXNA(newSelection)
{
	document.getElementById("data").innerHTML = newSelection["someString"] + " " + newSelection["someInt"];
}
