﻿namespace WebAPIUdemy.DTOs.AuthDTO;

public class TokenModel
{
    public string? AcessToken { get; set; }
    public string? RefreshToken { get; set; }
}
