$(document).ready(function() {

	$("#textarea").on('keyup',function(e) {
		let text = $(this).val();
		$("#wordcount").html(text.length + " / 200");
		console.log(text.length);
	    $("#hidden").val(text); //store content to input[type=hidden]
	});


	
	$(".linkValue").on('input',function(e) {
		let parent = $(this).parent();
	    $(parent).find(".changeLink").val($(parent).find(".linkType").val() + "," + $(this).val()); //store content to input[type=hidden]
	});

	//optional - one line but wrap it
	/*$("#multilineinput").on('keypress',function(e) {    
	    if(e.which == 13) { //on enter
	        e.preventDefault(); //disallow newlines     
	        // here comes your code to submit
	    }
	});
	*/
});