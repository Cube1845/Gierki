import { Component } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthApiService } from '../../services/auth-api.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login-panel',
  standalone: true,
  imports: [ ReactiveFormsModule ],
  templateUrl: './login-panel.component.html',
  styleUrl: './login-panel.component.scss'
})
export class LoginPanelComponent {
  constructor(private readonly authApiService: AuthApiService, private readonly router: Router) {}

  userFormControl: FormGroup = new FormGroup({
    username: new FormControl("", [Validators.required]),
    password: new FormControl("", [Validators.required])
  });

  login(): void {
    this.authApiService.loginWithUsernameAndData(this.userFormControl.value).subscribe(result => {
      console.log(result);
      if (result.value != null) {
        this.goToLobby(result.value!);
      }
    });
  }

  goToLobby(token: string) {
    localStorage.setItem('token', token);
    this.router.navigateByUrl('lobby');
  }

  goToRegister(): void {
    this.router.navigateByUrl('/register');
  }
}
