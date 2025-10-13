import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { axiosInstance } from '@/lib/axios';
import type { ServiceDto } from '@/types/api';

export interface CreateServiceDto {
  serviceId?: number;
  serviceName: string;
  servicePrice: number;
}

// Get all services
export const useServices = () => {
  return useQuery({
    queryKey: ['services'],
    queryFn: async () => {
      const { data } = await axiosInstance.get<ServiceDto[]>('/api/service');
      return data;
    },
    refetchInterval: 15000, // Auto-refetch every 15 seconds
    refetchIntervalInBackground: false, // Only refetch when tab is active
  });
};

// Create service
export const useCreateService = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (service: CreateServiceDto) => {
      const { data } = await axiosInstance.post<ServiceDto>('/api/service', service);
      return data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['services'] });
    },
  });
};

// Update service
export const useUpdateService = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({ id, service }: { id: number; service: CreateServiceDto }) => {
      const { data } = await axiosInstance.put<ServiceDto>(`/api/service/${id}`, service);
      return data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['services'] });
    },
  });
};

// Delete service
export const useDeleteService = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (id: number) => {
      await axiosInstance.delete(`/api/service/${id}`);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['services'] });
    },
  });
};
