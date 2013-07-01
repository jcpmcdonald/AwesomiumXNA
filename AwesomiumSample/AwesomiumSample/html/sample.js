
// A useful helper function that will let you create your GUI in a web browser
function InXNA()
{
	return typeof xna !== 'undefined';
}


handleDataFromXNA = function(data)
{
	$("#data").text(data);
}



var mouseDownOverHUD = false;
function docMouseDown(event)
{
	if(InXNA())
	{
		xna.OnMouseDown(mouseDownOverHUD, event.which - 1);
	}
	mouseDownOverHUD = false;
}

var mouseUpOverHUD = false;
function docMouseUp(event)
{
	if(InXNA())
	{
		xna.OnMouseUp(mouseUpOverHUD, event.which - 1);
	}
	mouseUpOverHUD = false;
}


$(document).ready(function()
{
	// Make button presses look cool
	$(".button").mousedown( function(event)
	{
		$(this).addClass("buttonPressed");
	});
	
	$(".button").mouseup( function(event)
	{
		$(this).removeClass("buttonPressed");
		if(!$(this).hasClass("disabled"))
		{
			XNACall($(this));
		}
	});
	
	$(".button").mouseleave( function(event)
	{
		$(this).removeClass("buttonPressed");
	});
	
	
	
	// Capture mouse up/down events so we can tell XNA if we've handled the action or not
	$(document).mousedown( function(event)
	{
		docMouseDown(event);
	});
	
	$("body").mousedown( function(event)
	{
		mouseDownOverHUD = true;
	});
	
	$(document).mouseup( function(event)
	{
		docMouseUp(event);
	});
	
	$("body").mouseup( function(event)
	{
		mouseUpOverHUD = true;
	});
});


function XNACall(param)
{
	if(InXNA())
	{
		var callString;
		if (typeof param === 'string')
		{
			callString = param;
		}
		else if (typeof param !== 'undefined' && typeof param.attr("call") !== 'undefined')
		{
			callString = param.attr("call");
		}
		else
		{
			return;
		}
		
		eval(callString);
	}
}
