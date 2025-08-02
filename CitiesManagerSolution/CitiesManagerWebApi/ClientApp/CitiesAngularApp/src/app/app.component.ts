import { Component } from '@angular/core';
import { AccountService } from './services/account.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  constructor(public accountService: AccountService, private router: Router) { }

  onLogOutClicked() {
    this.accountService.getLogout().subscribe({
      next: (response: string) => {
        this.accountService.currentUserName = null;

        localStorage.removeItem('token');
        localStorage.removeItem('refreshToken');

        this.router.navigate([ '/login' ]);
      },
      error: (error) => {
        console.error('Logout failed', error);
      },
      complete: () => { }
    });

  }
}
