<?php
	include("includes/config.php");
	include("includes/classes/Account.php");
	include("includes/classes/Constants.php");

	$account = new Account($con);

	include("includes/handlers/register-handler.php");
	include("includes/handlers/login-handler.php");

	function getInputValue($name) {
		if(isset($_POST[$name])) {
			echo $_POST[$name] ;
		}
	}

?>

<!doctype html>
<html>
<head>
<meta charset="utf-8">
<title>MUZIKOS BANGA</title>
<link rel="stylesheet" href="assets/css/register.css">

<script src="https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js"></script>
<script src="assets/js/register.js"></script>
</head>
<body>
	<?php
	if(isset($_POST['registerButton'])) {
	echo '<script>
		$(document).ready(function() {
				$("#loginForm").hide();
				$("#registerForm").show();
		});
	</script>';
	} else {
	echo '<script>
		$(document).ready(function() {
				$("#loginForm").show();
				$("#registerForm").hide();
		});
	</script>';
	}
	?>

	<div id="background">
	  <div id="loginContainer">
		<div id="inputContainer">
				<form id="loginForm" action="" method="POST" >
					<h2>PRISIJUNGTI</h2>
					<p>
					<?php echo $account->getError( Constants:: $loginFailed ) ?>
					<label for="loginUsername">Naudotojo vardas</label>
					<input id="loginUsername" name="loginUsername" type="text" placeholder="Naudotojo vardas" value="<?php getInputValue('loginUsername') ?>" required>
					</p>
					<p>
					<label for="loginPassword">Slaptažodis</label>
					<input id="loginPassword" name="loginPassword" type="password" placeholder="Slaptažodis" required>
					</p>
					<button type="submit" name="loginButton">Prisijungti</button>
					<div class="hasAccountText">
						<span id="hideLogin">Neturi prisijungimo? Registracija čia. </span>
					</div> <!-- hasAccountText -->
				</form> <!-- form -->

			<form id="registerForm" action="" method="POST" >
					<h2>REGISTRACIJA</h2>
					<p>
					<?php echo $account->getError( Constants::$usernameTaken ) ?>
					<?php echo $account->getError( Constants::$usernameCharacters ) ?>
					<label for="username">Naudotojo vardas</label>
					<input id="username" name="username" type="text" placeholder="Naudotojo vardas" value="<?php getInputValue('username') ?>" required>
					</p>
					<p>
					<?php echo $account->getError( Constants:: $firstNameCharacters ) ?>
					<label for="firstName">Vardas</label>
					<input id="firstName" name="firstName" type="text" placeholder="Vardas" value="<?php getInputValue('firstName') ?>" required>
					</p>
					<p>
					<?php echo $account->getError( Constants:: $lastNameCharacters) ?>
					<label for="lastName">Pavardė</label>
					<input id="lastName" name="lastName" type="text" placeholder="Pavardė" value="<?php getInputValue('lastName') ?>" required>
					</p>
					<?php echo $account->getError( Constants:: $emailTaken ) ?>
					<?php echo $account->getError( Constants:: $emailsDoNotMatch ) ?>
					<?php echo $account->getError( Constants:: $emailInvalid ) ?>
					
					<label for="email">El. paštas</label>
					<input id="email" name="email" type="email" placeholder="El. paštas" value="<?php getInputValue('email') ?>" required>
					</p>
					<p>
					<label for="email2">Pakartoti el. paštą</label>
					<input id="email2" name="email2" type="email" placeholder="Pakartoti el. paštą" value="<?php getInputValue('email2') ?>" required>
					</p>
					<p>
					<?php echo $account->getError( Constants:: $passwordNotAlpahnumeric ) ?>
					<?php echo $account->getError( Constants:: $passwordCharacters ) ?>
					<?php echo $account->getError( Constants:: $passwordDoNoMatch ) ?>
					<label for="password2">Slaptažodis</label>
					<input id="password2" name="password2" type="password" placeholder="Slaptažodis" required>
					</p>
					<p>
					<label for="password">Pakartoti slaptažodį</label>
					<input id="password" name="password" type="password" placeholder="Pakartoti slaptažodį" required>
					</p>
					<button type="submit" name="registerButton">Prisijungti</button>
					<div class="hasAccountText">
						<span id="hideRegister">Turite prisijungimus? Prisijungti čia. </span>
				</div> <!-- hasAccountText -->
			</form> <!-- form -->
		   </div> <!-- inputContainer -->
		   
		   <div id="loginText">
		   		<h1 class="elegantshadow">"Muzikos banga"</h1>
		   		<h2></h2>
		   		<ul>
		   			<li>Dainų kokybė garantuota</li>
		   			<li>Susikursite savo grojaraštį</li>
		   			<li>Geros muzikos paieška</li>
		   		</ul>
		   </div>
		   
	   </div> <!-- loginContainer -->
	</div> <!-- background -->

</body>
</html>
