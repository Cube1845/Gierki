import { Component } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthApiService } from '../../services/auth-api.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register-panel',
  standalone: true,
  imports: [ ReactiveFormsModule ],
  templateUrl: './register-panel.component.html',
  styleUrl: './register-panel.component.scss'
})
export class RegisterPanelComponent {
  constructor(private readonly authApiService: AuthApiService, private readonly router: Router) {}

  userFormControl: FormGroup = new FormGroup({
    username: new FormControl("", [Validators.required]),
    email: new FormControl(null),
    password: new FormControl("", [Validators.required])
  });

  register(): void {
    this.authApiService.register(this.userFormControl.value).subscribe(result => {
      console.log(result);
      this.navigateToLoginPanel();
    });
    this.userFormControl.reset();
    
  }

  navigateToLoginPanel(): void {
    this.router.navigateByUrl('login');
    alert("Successfully registered, now log in");
  }
}
