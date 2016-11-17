import { Injectable } from '@angular/core';
import { Router } from '@angular/router';

import { DataService } from './data.service';
import { User } from '../models/user.model';

@Injectable()
export class AuthService {

    constructor(private router: Router) { }

    logout() {
        sessionStorage.clear();
        this.router.navigate(['/login']);
    }

    isLoggedIn(): boolean {
        return this.user(undefined) !== undefined;
    }

    user(user: User): User {
        if (user) {
            sessionStorage.setItem('user', JSON.stringify(user));
        }
        let userData = JSON.parse(sessionStorage.getItem('user'));
        if (userData) {
            user = new User(userData.displayName, userData.roles);
        }
        return user ? user : undefined;
    }

    setAuth(res: any): void {
        if (res && res.user) {
            sessionStorage.setItem('user', JSON.stringify(res.user));
        }
    }
}
