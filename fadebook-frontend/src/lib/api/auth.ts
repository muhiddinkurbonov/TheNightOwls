import { axiosInstance } from '../axios';
import type { LoginDto, RegisterDto, LoginResponseDto, UserDto, ChangePasswordDto } from '@/types/api';

export const authApi = {
  // Register a new user
  register: async (data: RegisterDto): Promise<LoginResponseDto> => {
    const response = await axiosInstance.post<LoginResponseDto>('/api/auth/register', data);
    return response.data;
  },

  // Login user
  login: async (data: LoginDto): Promise<LoginResponseDto> => {
    const response = await axiosInstance.post<LoginResponseDto>('/api/auth/login', data);
    return response.data;
  },

  // Get current user
  getCurrentUser: async (): Promise<UserDto> => {
    const response = await axiosInstance.get<UserDto>('/api/auth/me');
    return response.data;
  },

  // Change password
  changePassword: async (data: ChangePasswordDto): Promise<UserDto> => {
    const response = await axiosInstance.post<UserDto>('/api/auth/change-password', data);
    return response.data;
  },

  // Update profile
  updateProfile: async (data: Partial<UserDto>): Promise<UserDto> => {
    const response = await axiosInstance.put<UserDto>('/api/auth/profile', data);
    return response.data;
  },
};
