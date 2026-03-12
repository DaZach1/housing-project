import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);
  
  if (!authService.currentUserValue) {
    router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
    return false;
  }

  // Проверка ролей
  const requiredRoles = route.data?.['roles'] as Array<string>;
  if (requiredRoles && !requiredRoles.includes(authService.currentUserValue.role)) {
    router.navigate(['/']);
    return false;
  }

  return true;
};