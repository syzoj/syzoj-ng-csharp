import React, { Component } from 'react'
import Form from 'react-bootstrap/lib/Form'
import FormGroup from 'react-bootstrap/lib/FormGroup'
import FormControl from 'react-bootstrap/lib/FormControl'
import ControlLabel from 'react-bootstrap/lib/ControlLabel'
import Button from 'react-bootstrap/lib/Button'
import { request } from '../util'
import { ErrorList } from '../components/Error'
import { Redirect } from 'react-router-dom'

export class Register extends Component {
    constructor(props) {
        super(props)
        this.state = {userName: '', email: '', password: '', errors: null, success: false}
    }
    submit(e) {
        e.preventDefault()
        request("/api/auth/register", "post", {
            Email: this.state.email,
            UserName: this.state.userName,
            Password: this.state.password,
        }).then((response) => {
            console.log(response)
            this.setState({success: true})
        }).catch(e => {
            this.setState({errors: e})
        })
    }
    render() {
        if(this.state.success)
            return <Redirect to='/login' />
        return <div>
            <ErrorList errors={this.state.errors} />
            <Form onSubmit={(e) => this.submit(e)}>
                <FormGroup controlId="formBasicUserName">
                    <ControlLabel>User Name</ControlLabel>
                    <FormControl placeholder="Enter user name" onChange={(e) => this.setState({userName: e.target.value})} value={this.state.userName} />
                </FormGroup>
                <FormGroup controlId="formBasicEmail">
                    <ControlLabel>Email address</ControlLabel>
                    <FormControl type="email" placeholder="Enter email" onChange={(e) => this.setState({email: e.target.value})} value={this.state.email} />
                </FormGroup>
                <FormGroup controlId="formBasicPassword">
                    <ControlLabel>Password</ControlLabel>
                    <FormControl type="password" placeholder="Password" onChange={(e) => this.setState({password: e.target.value})} value={this.state.password} />
                </FormGroup>
                <Button variant="primary" type="submit">
                    Submit
                </Button>
            </Form>
        </div>;
    }
}