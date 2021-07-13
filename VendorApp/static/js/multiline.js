$(document).ready(function() {
	$("#textarea").on('keyup',function(e) {

		console.log($(this).val())

	    $("#hidden").val($(this).val()); //store content to input[type=hidden]
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